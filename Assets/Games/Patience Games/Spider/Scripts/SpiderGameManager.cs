namespace CardGameArchive.Solitaire.Spider
{
    using CardGameArchive.Solitaire.Klondike;
	using System.Collections.Generic;
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
			GameTaskManager.Instance.QueueTask(() => Task.Delay(250));

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
			
		}
		public override List<ZoneParent> GetPossibleMoves(Card card, bool simulation = false)
		{
			throw new System.NotImplementedException();
		}
		public override void AutoMoveAny() { } // Overriden purely to disable
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
			throw new System.NotImplementedException();
		}

		public override Task UndoMove()
		{
			throw new System.NotImplementedException();
		}
    }
}