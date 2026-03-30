namespace CardGameArchive.Solitaire.Klondike
{
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Linq;
	using static Deck;

	public class KlondikeGameManager : BaseGameManager
	{
		protected override void SetRules()
		{
			Rules = new KlondikeGameRules();
		}

		protected override async void StartGame()
		{
			gameBoard.GetZoneParents(GameBoard.CardZone.Stock)[0].GetComponent<DeckObject>().InitializeDeck(Deck);

			Deck.Initialise(DeckType.Full52, gameBoard.GetZoneParents(GameBoard.CardZone.Stock)[0].GetComponent<DeckObject>());
			Deck.Shuffle();

			List<Task> dealingTasks = new();

			for (int i = 0; i < 7; i++)
			{
				for (int j = i; j < 7; j++)
				{
					Card card = Deck.Draw();
					dealingTasks.Add(gameBoard.MoveCard
						(
							card: card,
							destination: GameBoard.CardZone.Tableau,
							index: j,
							fromStock: true,
							canUndo: false
						));

					// Last card in column
					if (j == i)
					{
						card.SetFlipped(true);
					}

					await Task.Delay(50);
				}
			}

			gameBoard.OnCardMoveStart += OnCardMoveStart;
		}

		public override async void OnCardTapped(Card card)
		{
			await AutoMove(card);

			if (Rules.IsWinConditionAchieved())
			{
				UIManager.Instance.ShowWinScreen();
			}
		}
		public override void OnCardGrabbed(Card card)
		{
			// Move card above everything else
			card.linkedObj.transform.position = new(card.linkedObj.transform.position.x, card.linkedObj.transform.position.y, -1);
		}
		public override void OnCardDropped(Card card)
		{
			// Decide if the card can be placed here, if not, move it back
			GameObject destination = null;
		}

		public override async void OnDeckTapped(Deck deck)
		{
			ZoneParent waste = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0];
			ZoneParent stock = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Stock)[0];

			waste.SetOperations(false);

			List<Task> tasks = new();

			if (deck.RemainingCards > 0)
			{
				for (int i = 0; i < 3; i++)
				{
					Card card = Deck.Draw();

					// This means we've reached the end of the deck
					if (card == null)
						break;

					tasks.Add(GameBoard.Instance.MoveCard(card, GameBoard.CardZone.Waste,
													fromStock: true,
													canUndo: false));

					card.SetFlipped(true);					
				}

				gameMoves.Push(new(GameMove.MoveType.CardsDrawn, new GameMove.CardsDrawnData(3)));
			}

			// Return all cards in waste into deck
			else
			{
				List<CardObject> cards = waste.transform.GetAllChildren().Select(o => o.GetComponent<CardObject>()).Where(o => o != null).ToList();

				cards.Reverse();

				gameMoves.Push(new(GameMove.MoveType.WasteRecycled, new GameMove.WasteRecycledData()));

				CardObject firstCard = cards[0];
				foreach (CardObject card in cards)
				{
					deck.AddCard(card.CardData);
					card.CardData.SetFlipped(false);
					tasks.Add(GameBoard.Instance.MoveCard(card.CardData, GameBoard.CardZone.Stock,
													canUndo: false));
				}
			}

			await Task.WhenAll(tasks);

			waste.SetOperations(true);
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

		/// <summary>
		/// Automatically move this card somewhere it will 
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public override async Task AutoMove(Card card)
		{
			List<ZoneParent> possibleParents = GetPossibleMoves(card);

			if (possibleParents.Count <= 0)
				return;

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
					Debug.Log($"Player moved card {eventData.card.Rank} of {eventData.card.Suit} from {eventData.from.ToString()} to {eventData.to.ToString()}");
				}
				
				if (eventData.from.BottomCard != null && eventData.from.BottomCard.Flipped == false)
				{
					if (eventData.canUndo)
					{
						gameMoves.Push(new(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(eventData.from.BottomCard, true, true)));
						Debug.Log($"Card {eventData.card.Rank} of {eventData.card.Suit} flipped face up"); 
					}
					eventData.from.BottomCard.SetFlipped(true);
				}
			}
		}

		public override async void UndoMove()
		{
			if (gameMoves.Count <= 0)
			{
				return;
			}
			GameMove move = gameMoves.Pop();

			switch (move.type)
			{
				case GameMove.MoveType.CardFlipped:
					GameMove.CardFlippedData flippedData = move.Data as GameMove.CardFlippedData;
					flippedData.cardData.SetFlipped(!flippedData.flipped);
					break;

				case GameMove.MoveType.CardMoved:
					GameMove.CardMovedData movedData = move.Data as GameMove.CardMovedData;
					GameBoard.Instance.MoveCard(movedData.cardData, movedData.from, canUndo: false);
					break;

				case GameMove.MoveType.CardsDrawn:
					GameMove.CardsDrawnData drawnData = move.Data as GameMove.CardsDrawnData;
					for (int i = 0; i < drawnData.cardsDrawn; i++)
					{
						Card card = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0].BottomCard;

						if (card == null)
							break;

						GameBoard.Instance.MoveCard(card, GameBoard.CardZone.Stock, canUndo: false);
						card.SetFlipped(false);
					}
					break;

				case GameMove.MoveType.WasteRecycled:
					while (Deck.RemainingCards > 0)
					{
						Card card = Deck.Draw();
						GameBoard.Instance.MoveCard(card, GameBoard.CardZone.Waste, fromStock: true, canUndo: false);
					}
					break;
			}

			if (move.Contingent)
			{
				UndoMove();
			}
		}

		private void OnDisable()
		{
			if (gameBoard != null)
				gameBoard.OnCardMoveStart -= OnCardMoveStart;
		}
	}
}
