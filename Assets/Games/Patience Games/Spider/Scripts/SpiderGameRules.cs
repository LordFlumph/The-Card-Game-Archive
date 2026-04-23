namespace CardGameArchive.Solitaire.Spider
{
	using System.Collections.Generic;

	public class SpiderGameRules : BaseGameRules
    {
		public override bool CanCardMove(Card card)
		{
			if (card?.linkedObj == null)
				return false;

			if (!card.Flipped)
				return false;

			if (!card.Interactable)
				return false;

			ZoneParent cardZoneParent = card.GetZoneParent();

			if (cardZoneParent == null)
				return false;

			return GameBoard.Instance.GetCardChain(card)[^1] == cardZoneParent.BottomCard;
		}

		public override int GetRankValue(Card.CardRank rank)
		{
			return rank switch
			{
				Card.CardRank.Ace => 1,
				Card.CardRank.Two => 2,
				Card.CardRank.Three => 3,
				Card.CardRank.Four => 4,
				Card.CardRank.Five => 5,
				Card.CardRank.Six => 6,
				Card.CardRank.Seven => 7,
				Card.CardRank.Eight => 8,
				Card.CardRank.Nine => 9,
				Card.CardRank.Ten => 10,
				Card.CardRank.Jack => 11,
				Card.CardRank.Queen => 12,
				Card.CardRank.King => 13,
				_ => throw new System.ArgumentOutOfRangeException("Unexpected rank value")
			};
		}

		public override bool IsWinConditionAchieved()
		{
			List<ZoneParent> parents = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
			foreach (var parent in parents)
			{
				if (parent.CardCount != 13)
					return false;
			}

			return true;
		}

		protected override bool IsTableauMoveValid(Card card, ZoneParent destination, Card parentCard = null, bool simulation = false)
		{
			throw new System.NotImplementedException();
		}
    }

}