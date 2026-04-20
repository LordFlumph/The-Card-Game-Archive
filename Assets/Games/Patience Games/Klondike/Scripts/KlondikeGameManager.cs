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
				deck.Shuffle(false);
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

			GameTaskManager.Instance.AddTask(gameBoard.GenerateCards());
			GameTaskManager.Instance.QueueTask(() => Task.Delay(250));

			await GameTaskManager.Instance.WhenAll();

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
					GameTaskManager.Instance.AddTask(zone.BottomCard.SetFlipped(false));
					GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(zone.BottomCard, GameBoard.CardZone.Stock, canUndo: false, affectCardChain: false));
				}
			}

			gameMoves.Clear();

			GameTaskManager.Instance.QueueTask(() => Task.Delay(200));
			await GameTaskManager.Instance.WhenAll();

			base.RestartGame();
		}

		public override async void OnCardTapped(Card card)
		{
			GameTaskManager.Instance.AddTask(AutoMove(card));
			await GameTaskManager.Instance.WhenAll();

			if (Rules.IsWinConditionAchieved())
			{
				InputManager.Instance.DisableInput();
				UIManager.Instance.ShowWinScreenAsync();
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

				if (IsGameStuck())
				{
					UIManager.Instance.ShowGameStuck();
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

		public override List<ZoneParent> GetPossibleMoves(Card card, bool simulation = false)
		{
			if (!simulation && !Rules.CanCardMove(card))
				return new();

			List<ZoneParent> possibleParents = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
			possibleParents.AddRange(GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau));

			for (int i = 0; i < possibleParents.Count; i++)
			{
				if (!Rules.IsMoveValid(card, possibleParents[i], simulation))
				{
					possibleParents.RemoveAt(i);
					i--;
				}
			}

			return possibleParents;
		}

		public override void AutoMoveAny()
		{
			List<(Card card, ZoneParent destination)> possibleMoves = new();

			List<Card> cardsToCheck = gameBoard.GetZoneParents(GameBoard.CardZone.Tableau).Select(o => o.BottomCard).ToList();
			cardsToCheck.Add(gameBoard.GetZoneParents(GameBoard.CardZone.Waste)[0].BottomCard);

			foreach (Card card in cardsToCheck)
			{
				if (card != null)
				{
					List<ZoneParent> validMoves = GetPossibleMoves(card).Where(o => o.Zone == GameBoard.CardZone.Foundation).ToList();
					if (validMoves.Count > 0)
					{
						possibleMoves.Add((card, validMoves[0]));
					}
				}
			}

			// Determine if a move can be done safely (taking into account potential future uses for the card)
			Card.CardRank clubRank = Card.CardRank.Ace;
			Card.CardRank diamondRank = Card.CardRank.Ace;
			Card.CardRank heartRank = Card.CardRank.Ace;
			Card.CardRank spadeRank = Card.CardRank.Ace;

			foreach (ZoneParent foundation in gameBoard.GetZoneParents(GameBoard.CardZone.Foundation))
			{
				if (foundation.BottomCard != null)
				{
					switch (foundation.BottomCard.Suit)
					{
						case Card.CardSuit.Clubs:
							clubRank = foundation.BottomCard.Rank;
							break;
						case Card.CardSuit.Diamonds:
							diamondRank = foundation.BottomCard.Rank;
							break;
						case Card.CardSuit.Hearts:
							heartRank = foundation.BottomCard.Rank;
							break;
						case Card.CardSuit.Spades:
							spadeRank = foundation.BottomCard.Rank;
							break;
					}
				}
			}
		
			for (int i = possibleMoves.Count - 1; i >= 0; i--)
			{
				// We can always safely automove aces and twos
				if (possibleMoves[i].card.Rank is Card.CardRank.Ace or Card.CardRank.Two)
				{
					continue;
				}

				// We only want to move a card if doing so will never affect any future moves. So make sure that there is no other use for this card.
				// For example, if we are checking a black 5, then we only want to move it if all the red 4s are in the foundation, as that way,
				// there is definitely no use for the black 5 outside of the foundation
				if (Card.SuitColours[possibleMoves[i].card.Suit] == Card.CardColour.Red)
				{
					if (Rules.GetRankValue(possibleMoves[i].card.Rank) - Rules.GetRankValue(clubRank) > 1 ||
						Rules.GetRankValue(possibleMoves[i].card.Rank) - Rules.GetRankValue(spadeRank) > 1)
					{
						possibleMoves.RemoveAt(i);
					}
				}
				else
				{
					if (Rules.GetRankValue(possibleMoves[i].card.Rank) - Rules.GetRankValue(diamondRank) > 1 ||
						Rules.GetRankValue(possibleMoves[i].card.Rank) - Rules.GetRankValue(heartRank) > 1)
					{
						possibleMoves.RemoveAt(i);
					}
				}
			}

			if (possibleMoves.Count > 0)
			{
				possibleMoves = possibleMoves.OrderBy(o => o.card.Rank).ToList();
				GameTaskManager.Instance.AddTask(gameBoard.MoveCard(possibleMoves[0].card, possibleMoves[0].destination, forceContingent: true));
				GameTaskManager.Instance.QueueTask(() => Task.Delay(250));
			}
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
				GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, possibleParents[0]));
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

			GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, highestParent));
		}
		public override bool IsGameStuck()
		{
			List<Card> cardsToCheck = gameBoard.GetZoneParents(GameBoard.CardZone.Tableau).Select(o => o.BottomCard).ToList();
			
			// Can we move any of the currently visible cards?
			foreach (Card card in cardsToCheck)
			{
				if (card != null)
				{
					if (GetPossibleMoves(card).Count > 0)
					{
						return false;
					}
				}
			}

			// Check if there are any possible moves if we draw cards
			// This function only runs when there are no cards in the waste, so no need to check waste
			cardsToCheck.Clear();
			Deck deck = gameBoard.GetDeck();
			for (int i = 2; i < deck.RemainingCards;)
			{
				cardsToCheck.Add(deck.Cards[i]);
				i+= 3;

				if (i >= deck.RemainingCards && deck.RemainingCards % 3 != 0)
				{
					cardsToCheck.Add(deck.Cards[^1]);
				}
			}

			foreach (Card card in cardsToCheck)
			{
				if (card != null)
				{
					if (GetPossibleMoves(card, true).Count > 0)
					{
						return false;
					} 
				}
			}


			return true;
		}

		protected override async void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from != null)
			{
				if (eventData.canUndo)
				{
					gameMoves.Push(new(GameMove.MoveType.CardMoved, new GameMove.CardMovedData(eventData.card, eventData.from, eventData.to, eventData.contingent)));
				}

				if (eventData.from.BottomCard != null)
				{
					if (eventData.from.Zone != GameBoard.CardZone.Stock)
					{
						if (eventData.from.BottomCard.Flipped == false)
						{
							GameTaskManager.Instance.AddTask(eventData.from.BottomCard.SetFlipped(true));
							if (eventData.canUndo)
							{
								gameMoves.Push(new(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(eventData.from.BottomCard, true, true)));
							}
						}

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
					UIManager.Instance.HideGameStuck();
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
					UIManager.Instance.HideGameStuck();
					break;
			}

			if (move.Contingent)
			{
				UndoMove();
			}

			await GameTaskManager.Instance.WhenAll();
		}


		[System.Serializable]
		public class KlondikeSaveData : BaseGameSaveData
		{
			public List<SaveData> gameMoves = new();

			public KlondikeSaveData(float gameTime) : base(gameTime) { }
		}

		public override SaveData Save()
		{
			KlondikeSaveData data = new(GameTime);
			foreach (GameMove move in gameMoves)
			{
				data.gameMoves.Add(move.Save());
			}
			return data;
		}

		public override void Load(SaveData saveData)
		{
			base.Load(saveData);

			KlondikeSaveData klondikeData = (saveData as GameSaveData).gameManagerData as KlondikeSaveData;

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
