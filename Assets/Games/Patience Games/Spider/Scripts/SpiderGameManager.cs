namespace CardGameArchive.Solitaire.Spider
{
	using CardGameArchive.Solitaire.Klondike;
	using NUnit.Framework;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using UnityEngine;

	public class SpiderGameManager : KlondikeGameManager
	{
		[SerializeField] GameTerms.GameName gameName;
		protected override void SetGame()
		{
			Rules = new SpiderGameRules();
			Name = gameName;
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

			Debug.Log("Verification attempts: " + verificationCounter);
			if (verificationCounter >= 50)
			{
				Debug.LogWarning("Failed to verify deck");
			}

			GameTaskManager.Instance.AddTask(gameBoard.GenerateCards());
			GameTaskManager.Instance.QueueTask(() => Task.Delay(500));

			await GameTaskManager.Instance.WhenAll();

			int dealCount = 54;
			while (dealCount > 0)
			{
				for (int i = 0; i < 10; i++)
				{
					if (dealCount <= 0)
						break;

					Card card = deck.Draw();
					card.SetInteractable(false);

					GameTaskManager.Instance.AddTask(gameBoard.MoveCard(
						card: card,
						destination: GameBoard.CardZone.Tableau,
						index: i,
						timeToMove: 0.15f,
						canUndo: false,
						affectCardChain: false));

					if (dealCount <= 10)
					{
						card.SetInteractable(true);
						GameTaskManager.Instance.AddTask(card.SetFlipped(true));
					}

					await Awaitable.WaitForSecondsAsync(0.05f);

					dealCount--;
				}
			}

			await GameTaskManager.Instance.WhenAll();

			foreach (ZoneParent parent in gameBoard.GetZoneParents(GameBoard.CardZone.Tableau))
			{
				Card card = parent.BottomCard;

				card.SetInteractable(true);
			}
		}

		protected override bool VerifyDeck()
		{
			Deck deck = gameBoard.GetDeck();
			List<Card> visibleCards = deck.Cards.GetRange(deck.Cards.Count-54, 10);

			// Confirm there are at least 2 useful moves (same suit)
			int usefulMoves = 0;
			foreach (Card card in visibleCards)
			{
				foreach (Card targetCard in visibleCards)
				{
					if (card == targetCard)
						continue;

					if (card.Suit == targetCard.Suit)
					{
						if (Rules.GetRankValue(targetCard) - Rules.GetRankValue(card) == 1)
						{
							usefulMoves++;
						}
					}
				}
			}

			if (usefulMoves < 3)
				return false;


			// Confirm that there are no more than 3 of the same Rank present			
			if (visibleCards.GroupBy(o => o.Rank).Any(o => o.Count() > 3))
				return false;


			// Confirm that there is between 4 and 7 of the same suit are present in visible cards
			if (!(visibleCards.GroupBy(o => o.Suit).Any(o => o.Count() is >= 4 and <= 7)))
				return false;


			// Confirm that there is no huge gap between card ranks
			visibleCards = visibleCards.OrderBy(o => Rules.GetRankValue(o)).ToList();
			for (int i = 1; i < visibleCards.Count; i++)
			{
				if (Rules.GetRankValue(visibleCards[i]) - Rules.GetRankValue(visibleCards[i-1]) > 6)
				{
					return false;
				}
			}

			// Lastly, confirm that every deal has at least 1 good move, and 2 possible moves

			usefulMoves = 0;
			int possibleMoves = 0;
			List<List<Card>> deals = new();
			deals.Add(new List<Card>(deck.Cards.GetRange(0, 10)));
			deals.Add(new List<Card>(deck.Cards.GetRange(10, 10)));
			deals.Add(new List<Card>(deck.Cards.GetRange(20, 10)));
			deals.Add(new List<Card>(deck.Cards.GetRange(30, 10)));
			deals.Add(new List<Card>(deck.Cards.GetRange(40, 10)));
			foreach (var deal in deals)
			{
				usefulMoves = 0;
				possibleMoves = 0;
				foreach (Card card in deal)
				{
					foreach (Card targetCard in deal)
					{
						if (card == targetCard)
							continue;

						if (Rules.GetRankValue(targetCard) - Rules.GetRankValue(card) == 1)
						{
							possibleMoves++;
							if (card.Suit == targetCard.Suit)
							{
								usefulMoves++;
							}

							if (possibleMoves >= 2 && usefulMoves >= 1)
								break;							
						}
						
					}
					if (possibleMoves >= 2 && usefulMoves >= 1)
						break;
				}

				if (possibleMoves < 2 || usefulMoves < 1)
					return false;
			}

			return true;
		}
		public override async void OnDeckTapped(Deck deck)
		{
			if (deck.RemainingCards > 0)
			{
				// Check if all Tableaus are occupied
				List<ZoneParent> tableauParents = gameBoard.GetZoneParents(GameBoard.CardZone.Tableau);
				if (!tableauParents.All(o => o.CardCount > 0))
				{
					// All tableau must contain cards
					foreach (ZoneParent parent in tableauParents)
					{
						if (parent.CardCount == 0)
						{
							FeedbackManager.Instance.PulseHighlight(parent.gameObject, FeedbackManager.Instance.InvalidColour);
						}
					}
					InvokeInvalidAction(null);
					return;
				}
				foreach (ZoneParent parent in tableauParents)
				{
					Card card = deck.Draw();
					GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, parent, canUndo: false, affectCardChain: false));
					GameTaskManager.Instance.AddTask(card.SetFlipped(true));
					card.SetInteractable(true);
					await Awaitable.WaitForSecondsAsync(0.05f);
				}

				gameMoves.Push(new GameMove(GameMove.MoveType.CardsDrawn, new GameMove.CardsDrawnData(10)));
			}
		}
		public override List<ZoneParent> GetPossibleMoves(Card card, bool simulation = false)
		{
			if (!simulation && !Rules.CanCardMove(card))
				return new();

			List<ZoneParent> possibleMoves = new();

			foreach (ZoneParent tableau in gameBoard.GetZoneParents(GameBoard.CardZone.Tableau))
			{
				if (Rules.IsMoveValid(card, tableau, simulation))
				{
					possibleMoves.Add(tableau);
				}
			}

			return possibleMoves;
		}
		public override async void AutoMoveAny()
		{
			foreach (Card card in gameBoard.GetZoneParents(GameBoard.CardZone.Tableau).Select(o => o.BottomCard).Where(o => o != null))
			{
				if (card.Rank == Card.CardRank.Ace)
				{
					List<Card> cardChain = Rules.GetCardChain(card);
					if (cardChain?.Count == 13)
					{
						foreach (ZoneParent foundation in gameBoard.GetZoneParents(GameBoard.CardZone.Foundation))
						{
							if (foundation.CardCount == 0)
							{
								cardChain.Reverse();
								foreach (Card chainCard in cardChain)
								{
									await Awaitable.WaitForSecondsAsync(0.05f);
									GameTaskManager.Instance.AddTask(gameBoard.MoveCard(chainCard, foundation, forceContingent: true));
								}

								return;
							}
						}
						Debug.LogError("Attempting to move a complete stack to foundation but no foundation remains");
					}					
				}
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
			possibleParents = possibleParents.OrderByDescending(o => Rules.GetCardChain(o).Count).ToList();
			foreach (ZoneParent parent in possibleParents)
			{
				if (parent.CardCount > 0)
				{
					if (parent.BottomCard.Suit == card.Suit)
					{
						GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, parent));
						return;
					}
				}
			}

			GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, possibleParents[0]));
		}
		public override bool IsGameStuck()
		{
			if (gameBoard.GetDeck().RemainingCards > 0)
				return false;

			List<Card> cardsToCheck = new();

			foreach (Card card in gameBoard.GetZoneParents(GameBoard.CardZone.Tableau).Select(o => o.BottomCard))
			{
				if (card != null)
					cardsToCheck.AddRange(Rules.GetCardChain(card));
				else // If there is an empty tableau space, then there is always a valid move
					return false;
			}

			foreach (Card card in cardsToCheck)
			{
				List<ZoneParent> possibleMoves = GetPossibleMoves(card);
				if (possibleMoves.Count > 0)
				{
					foreach (ZoneParent parent in possibleMoves)
					{
						// Check if moving would actually affect the game state
						if (card.linkedObj.TryGetParentCard(out CardObject parentCard))
						{
							Card newParentCard = parent.BottomCard;

							if (parentCard.Flipped == false)
								return false;

							// If we are moving to a different rank or suit, then we have a valid move
							if (newParentCard.Rank != parentCard.Rank || (newParentCard.Suit != parentCard.Suit && newParentCard.Suit == card.Suit))
							{
								return false;
							}
						}
						else // This card is at the bottom of the tableau, so moving it will free up space
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		protected override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.canUndo)
			{
				gameMoves.Push(new(GameMove.MoveType.CardMoved, new GameMove.CardMovedData(eventData.card, eventData.from, eventData.to, eventData.contingent)));
			}

			if (eventData.from != null)
			{
				if (eventData.from.Zone == GameBoard.CardZone.Tableau)
				{
					if (eventData.from.CardCount > 0)
					{
						Card fromCard = eventData.from.BottomCard;
						if (!fromCard.Flipped)
						{
							GameTaskManager.Instance.AddTask(fromCard.SetFlipped(true));
							gameMoves.Push(new(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(fromCard, true, true)));
						}
						
						foreach (Card card in Rules.GetCardChain(fromCard))
						{
							if (card.Flipped)
								card.SetInteractable(true);
						}
					}
				}
			}
			if (eventData.to != null)
			{
				if (eventData.to.Zone == GameBoard.CardZone.Foundation)
				{
					eventData.card.SetInteractable(false, false);
				}
			}
		}

		protected override void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.to != null)
			{
				if (eventData.to.Zone == GameBoard.CardZone.Tableau)
				{
					// Disable all cards in parent
					foreach (Card card in eventData.to.Cards.Select(o => o.Data))
					{
						card.SetInteractable(false);
					}

					// Only re-enable those in the card chain
					foreach (Card card in Rules.GetCardChain(eventData.to))
					{
						card.SetInteractable(true);
					}
				}
			}

			if (IsGameStuck())
			{
				UIManager.Instance.ShowGameStuck();
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
					GameTaskManager.Instance.AddTask(gameBoard.MoveCard(movedData.cardData, movedData.from, canUndo: false));
					break;

				case GameMove.MoveType.CardsDrawn:
					GameMove.CardsDrawnData drawnData = move.Data as GameMove.CardsDrawnData;
					List<ZoneParent> tableauParents = gameBoard.GetZoneParents(GameBoard.CardZone.Tableau);
					tableauParents.Reverse();
					foreach (ZoneParent tableau in tableauParents)
					{
						Card card = tableau.BottomCard;

						GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, GameBoard.CardZone.Stock, canUndo: false));
						gameBoard.GetDeck().AddCard(card);
						GameTaskManager.Instance.AddTask(card.SetFlipped(false));
						card.SetInteractable(false);
						await Awaitable.WaitForSecondsAsync(0.05f);
					}
					break;
				default:
					throw new System.NotImplementedException("Attempted to undo a move that hasn't been accounted for");
			}

			UIManager.Instance.HideGameStuck();

			if (move.Contingent)
			{
				if (move.type == GameMove.MoveType.CardMoved && gameMoves.Peek().type == GameMove.MoveType.CardMoved)
					await Awaitable.WaitForSecondsAsync(50);
				UndoMove();
			}

			await GameTaskManager.Instance.WhenAll();
		}

		public override void Load(SaveData saveData)
		{
			base.Load(saveData);

			if (!loadFailed)
			{
				foreach (ZoneParent parent in gameBoard.GetZoneParents(GameBoard.CardZone.Foundation))
				{
					foreach (CardObject card in parent.Cards)
					{
						FeedbackManager.Instance.EnableCard(card);
					}
				}
			}
		}
	}
}