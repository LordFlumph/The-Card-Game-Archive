namespace CardGameArchive.Solitaire.Spider
{
    using CardGameArchive.Solitaire.Klondike;
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
			GameTaskManager.Instance.AddTask(gameBoard.GenerateCards());
			GameTaskManager.Instance.QueueTask(() => Task.Delay(500));

			await GameTaskManager.Instance.WhenAll();

			Deck deck = gameBoard.GetDeck();

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
						GameTaskManager.Instance.AddTask(card.SetFlipped(true));
					}

					await Task.Delay(50);

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

		protected override bool VerifyDeck() => true;
		public override void OnDeckTapped(Deck deck)
		{
			if (deck.RemainingCards > 0)
			{
				foreach (ZoneParent parent in gameBoard.GetZoneParents(GameBoard.CardZone.Tableau))
				{
					Card card = deck.Draw();
					GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, parent, canUndo: false, affectCardChain: false));
				}

				gameMoves.Push(new GameMove(GameMove.MoveType.CardsDrawn, new GameMove.CardsDrawnData(10)));
			}
		}
		public override List<ZoneParent> GetPossibleMoves(Card card, bool simulation = false)
		{
			throw new System.NotImplementedException();
		}
		public override void AutoMoveAny()
		{
			// Handle auto moving cards to foundation once a stack is complete
			throw new System.NotImplementedException();
		}
		public override async Task AutoMove(Card card, bool playerDriven = true)
		{
			throw new System.NotImplementedException();
		}
		public override bool IsGameStuck()
		{
			throw new System.NotImplementedException();
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
					Card fromCard = eventData.from.BottomCard;
					if (fromCard != null)
					{
						if (!fromCard.Flipped)
						{
							fromCard.SetInteractable(true);
							GameTaskManager.Instance.AddTask(fromCard.SetFlipped(true));
							gameMoves.Push(new(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(fromCard, true, true)));
						}
					}
				}
			}
			if (eventData.to != null)
			{
				if (eventData.to.Zone == GameBoard.CardZone.Foundation)
				{
					eventData.card.SetInteractable(false);
				}
				else if (eventData.to.Zone == GameBoard.CardZone.Tableau)
				{
					// Disable all cards in parent (will be re-enabled in OnCardMoveFinish)
					foreach (Card card in eventData.to.Cards.Select(o => o.Data))
					{
						card.SetInteractable(false);
					}
				}
			}			
		}

		protected override void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.to != null)
			{
				if (eventData.to.Zone == GameBoard.CardZone.Tableau)
				{
					// Only re-enable those in the card chain
					foreach (Card card in Rules.GetCardChain(eventData.card))
					{
						card.SetInteractable(true);
					}
				}
			}
		}

		public override Task UndoMove()
		{
			throw new System.NotImplementedException();
		}
    }
}