namespace CardGameArchive.Solitaire.Klondike
{
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Linq;
	using static Deck;

	public class KlondikeGameManager : BaseGameManager
	{
		protected override void SetGame()
		{
			Rules = new KlondikeGameRules();
			Name = GameTerms.GameName.Klondike;
		}
		protected override async Task StartGame()
		{
			int verificationCounter = 1;
			Deck deck = gameBoard.GetDeck();
			while (verificationCounter < 50)
			{
				deck.Shuffle();
				if (VerifyDeck())
				{
					break;
				}
				verificationCounter++;
			}

			if (verificationCounter >= 50)
			{
				Debug.LogWarning("Failed to verify deck");
			}

			gameBoard.GenerateCards();

			await Task.Delay(250);

			for (int i = 0; i < 7; i++)
			{
				for (int j = i; j < 7; j++)
				{
					Card card = deck.Draw();
					GameTaskManager.Instance.AddTask(gameBoard.MoveCard
						(
							card: card,
							destination: GameBoard.CardZone.Tableau,
							index: j,
							fromStock: true,
							timeToMove: 0.15f,
							canUndo: false
						));

					// Last card in column
					if (j == i)
					{
						GameTaskManager.Instance.AddTask(card.SetFlipped(true));
					}

					await Task.Delay(50);
				}
			}

			await GameTaskManager.Instance.WhenAll();

			// Set all but the bottom cards as uninteractable
			foreach (ZoneParent tableauParent in gameBoard.GetZoneParents(GameBoard.CardZone.Tableau))
			{
				foreach (CardObject card in tableauParent.Cards)
				{
					card.Data.SetInteractable(false);
				}
				tableauParent.BottomCard.SetInteractable(true);
			}
		}

		protected override bool VerifyDeck()
		{
			List<Card> cards = new List<Card>();
			List<List<Card>> tableau = new() { new(), new(), new(), new(), new(), new(), new() };
			Deck deck = gameBoard.GetDeck();

			while (deck.RemainingCards > 0)
			{
				cards.Add(deck.Draw());
			}

			for (int i = cards.Count - 1; i >= 0; i--)
			{
				deck.AddCard(cards[i]);
			}

			for (int i = 0; i < 7; i++)
			{
				for (int j = i; j < 7; j++)
				{
					Card card = cards[0];
					cards.RemoveAt(0);

					tableau[j].Add(card);
				}
			}

			// Fail if there are no immediate moves for the Tableau
			List<Card> lastCards = tableau.Select(o => o[^1]).ToList();
			bool validMoves = false;
			for (int i = 0; i < lastCards.Count; i++)
			{
				for (int j = i + 1; j < lastCards.Count; j++)
				{
					if (Mathf.Abs(Rules.GetRankValue(lastCards[i]) - Rules.GetRankValue(lastCards[j])) == 1 &&
						Card.SuitColours[lastCards[i].Suit] != Card.SuitColours[lastCards[j].Suit])
					{
						validMoves = true;
						break;
					}
				}

				if (validMoves)
				{
					break;
				}
			}

			if (!validMoves)
			{
				return false;
			}


			// Fail if any aces are buried deep in the tableau
			foreach (List<Card> tableauCards in tableau)
			{
				List<Card> aces = tableauCards.Where(o => o.Rank == Card.CardRank.Ace).ToList();
				if (aces.Count > 0)
				{
					foreach (Card ace in aces)
					{
						if (tableauCards.Count - tableauCards.IndexOf(ace) >= 3)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public override async void RestartGame()
		{
			List<ZoneParent> allZones = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
			allZones.AddRange(GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau));
			allZones.AddRange(GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste));

			foreach (ZoneParent zone in allZones)
			{
				while (zone.BottomCard != null)
				{
					GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(zone.BottomCard, GameBoard.CardZone.Stock, canUndo: false, affectCardChain: false));
				}
			}

			gameMoves.Clear();

			GameTaskManager.Instance.QueueTask(() => Task.Delay(200));
			await GameTaskManager.Instance.WhenAll();

			GameSceneManager.Instance.ReloadScene();
			return;
		}

		public override async void OnCardTapped(Card card)
		{
			GameTaskManager.Instance.AddTask(AutoMove(card));
			await GameTaskManager.Instance.WhenAll();

			if (Rules.IsWinConditionAchieved())
			{
				InputManager.Instance.DisableInput();
				UIManager.Instance.ShowWinScreen();
			}
		}
		public override void OnCardDropped(Card card)
		{
			bool actionExecuted = false;

			List<GameObject> objectsToIgnore = new();
			objectsToIgnore.AddRange(GameBoard.Instance.GetCardChain(card).Select(o => o.linkedObj.gameObject));

			RaycastHit2D[] hits = Physics2D.RaycastAll(card.linkedObj.transform.position, Vector3.forward, GameBoard.TopCardZ * 2);
			foreach (var hit in hits.OrderBy(o => o.distance))
			{
				if (objectsToIgnore.Contains(hit.collider.gameObject))
					continue;

				if (hit.collider.TryGetComponent(out ZoneParent zoneParent))
				{
					if (zoneParent.CardCount == 0 && zoneParent != card.GetZoneParent())
					{
						if (Rules.IsMoveValid(card, zoneParent))
						{
							actionExecuted = true;
							gameBoard.MoveCard(card, zoneParent);
						}
					}
					break;
				}

				else if (hit.collider.TryGetComponent(out CardObject otherCard))
				{
					ZoneParent otherZoneParent = otherCard.GetZoneParent();
					if (otherCard.Data == otherZoneParent.BottomCard)
					{
						if (Rules.IsMoveValid(card, otherZoneParent))
						{
							actionExecuted = true;
							gameBoard.MoveCard(card, otherZoneParent);
						}
					}
					break;
				}
			}

			if (!actionExecuted)
				InvokeInvalidAction(card);
		}

		public override async void OnDeckTapped(Deck deck)
		{
			ZoneParent waste = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0];
			ZoneParent stock = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Stock)[0];

			if (deck.RemainingCards > 0)
			{
				for (int i = 0; i < 3; i++)
				{
					Card card = deck.Draw();

					// This means we've reached the end of the deck
					if (card == null)
						break;

					card.SetInteractable(false);

					GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, GameBoard.CardZone.Waste,
													fromStock: true,
													canUndo: false));

					GameTaskManager.Instance.AddTask(card.SetFlipped(true));
				}

				gameMoves.Push(new(GameMove.MoveType.CardsDrawn, new GameMove.CardsDrawnData(3)));
			}

			// Return all cards in waste into deck
			else
			{
				List<CardObject> cards = waste.Cards;

				cards.Reverse();

				gameMoves.Push(new(GameMove.MoveType.ZoneTransfer, new GameMove.ZoneTransferData(waste, stock)));

				CardObject firstCard = cards[0];
				foreach (CardObject card in cards)
				{
					deck.AddCard(card.Data);
					card.Data.SetInteractable(false);
					GameTaskManager.Instance.AddTask(card.Data.SetFlipped(false));
					GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card.Data, GameBoard.CardZone.Stock,
													canUndo: false));
				}
			}

			await GameTaskManager.Instance.WhenAll();

			foreach (CardObject card in waste.Cards)
			{
				card.Data.SetInteractable(false);
			}

			waste.BottomCard?.SetInteractable(true);
			//waste.SetOperations(true);
		}

		public override List<ZoneParent> GetPossibleMoves(Card card)
		{
			if (!Rules.CanCardMove(card))
				return new();

			List<ZoneParent> possibleParents = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
			possibleParents.AddRange(GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau));

			for (int i = 0; i < possibleParents.Count; i++)
			{
				if (!Rules.IsMoveValid(card, possibleParents[i]))
				{
					possibleParents.RemoveAt(i);
					i--;
				}
			}

			return possibleParents;
		}

		public override async Task AutoMoveCards()
		{
			List<Card> possibleCards = new();

			possibleCards.Add(gameBoard.GetZoneParents(GameBoard.CardZone.Waste)[0].BottomCard);
			possibleCards.AddRange(gameBoard.GetZoneParents(GameBoard.CardZone.Tableau).Select(o => o.BottomCard));
		}

		public override async Task AutoMove(Card card, bool playerDriven = true)
		{
			List<ZoneParent> possibleParents = GetPossibleMoves(card);

			if (possibleParents.Count <= 0)
			{
				if (playerDriven)
					InvokeInvalidAction(card);

				return;
			}


			// If we can move to the foundation, do so
			if (possibleParents[0].Zone == GameBoard.CardZone.Foundation)
			{
				GameBoard.Instance.MoveCard(card, possibleParents[0]);
				return;
			}

			// Work out which Tableau column is best to move to (has the longest card chain)
			int highestCardChain = 0;
			ZoneParent highestParent = possibleParents[0];
			foreach (ZoneParent parent in possibleParents)
			{
				List<Card> cardChain = GameBoard.Instance.GetCardChain(parent);
				if (cardChain.Count > highestCardChain)
				{
					highestCardChain = cardChain.Count;
					highestParent = parent;
				}
			}

			GameBoard.Instance.MoveCard(card, highestParent);
		}

		protected override async void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from != null)
			{
				if (eventData.canUndo)
				{
					gameMoves.Push(new(GameMove.MoveType.CardMoved, new GameMove.CardMovedData(eventData.card, eventData.from, eventData.to)));
				}

				if (eventData.from.BottomCard != null)
				{
					if (eventData.canUndo)
					{
						gameMoves.Push(new(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(eventData.from.BottomCard, true, true)));
					}

					if (eventData.from.Zone != GameBoard.CardZone.Stock)
					{
						GameTaskManager.Instance.AddTask(eventData.from.BottomCard.SetFlipped(true));
						eventData.from.BottomCard.SetInteractable(true);
					}						
				}
			}
		}

		public override async Task UndoMove()
		{
			if (gameMoves.Count <= 0)
			{
				return;
			}
			GameMove move = gameMoves.Pop();
			InvokeUndo(move);

			switch (move.type)
			{
				case GameMove.MoveType.CardFlipped:
					GameMove.CardFlippedData flippedData = move.Data as GameMove.CardFlippedData;
					GameTaskManager.Instance.AddTask(flippedData.cardData.SetFlipped(!flippedData.flipped));
					flippedData.cardData.SetInteractable(!flippedData.flipped);
					break;

				case GameMove.MoveType.CardMoved:
					GameMove.CardMovedData movedData = move.Data as GameMove.CardMovedData;
					GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(movedData.cardData, movedData.from, canUndo: false));
					break;

				case GameMove.MoveType.CardsDrawn:
					GameMove.CardsDrawnData drawnData = move.Data as GameMove.CardsDrawnData;
					for (int i = 0; i < drawnData.cardsDrawn; i++)
					{
						Card card = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0].BottomCard;

						if (card == null)
							break;

						GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, GameBoard.CardZone.Stock, canUndo: false));
						gameBoard.GetDeck().AddCard(card);
						GameTaskManager.Instance.AddTask(card.SetFlipped(false));
						card.SetInteractable(false);
					}
					break;

				case GameMove.MoveType.ZoneTransfer:
					Deck deck = GameBoard.Instance.GetDeck();
					while (deck.RemainingCards > 0)
					{
						Card card = deck.Draw();
						GameTaskManager.Instance.AddTask(card.SetFlipped(true));
						GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, GameBoard.CardZone.Waste, fromStock: true, canUndo: false));
					}
					GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0].BottomCard.SetInteractable(true);
					break;
			}

			if (move.Contingent)
			{
				GameTaskManager.Instance.AddTask(UndoMove());
			}

			await GameTaskManager.Instance.WhenAll();
		}


		[System.Serializable]
		public class KlondikeSaveData : SaveData
		{
			public List<SaveData> gameMoves = new();
		}

		public override SaveData Save()
		{
			// Save GameMoves
			KlondikeSaveData data = new();
			foreach (GameMove move in gameMoves)
			{
				data.gameMoves.Add(move.Save());
			}
			return data;
		}

		public override void Load(SaveData saveData)
		{
			GameSaveData gameData = saveData as GameSaveData;
			KlondikeSaveData klondikeData = gameData.gameManagerData as KlondikeSaveData;

			gameBoard.Load(gameData.gameBoardData);

			List<GameMove.GameMoveSaveData> moveSaveData = klondikeData.gameMoves.OfType<GameMove.GameMoveSaveData>().ToList();
			moveSaveData.Reverse();
			gameMoves.Clear();
			foreach (var moveData in moveSaveData)
			{
				GameMove gameMove = new GameMove();
				gameMove.Load(moveData);
				gameMoves.Push(gameMove);
			}
		}
	}
}
