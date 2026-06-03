namespace CardGameArchive
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	public class FreecellGameManager : KlondikeGameManager
	{
		protected override void SetGame()
		{
			Rules = new Rules.FreecellGameRules();
		}
		protected override async Task StartGame()
		{

		}
		protected override bool VerifyDeck() => true;
		public override List<ZoneParent> GetPossibleMoves(Card card, bool simulation = false)
		{
			if (!simulation && !Rules.CanCardMove(card))
				return new();

			List<ZoneParent> possibleParents = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
			possibleParents.AddRange(GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau));
			possibleParents.AddRange(GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Cell));

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
		public override Task AutoMove(Card card, bool playerDriven = true)
		{
			return base.AutoMove(card, playerDriven);
		}

		public override void OnDeckTapped(Deck deck) { }
		public override bool IsGameStuck()
		{
			return false;
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

						foreach (Card card in Rules.GetCardChain(fromCard))
						{
							card.SetInteractable(true);
						}
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
	}
}