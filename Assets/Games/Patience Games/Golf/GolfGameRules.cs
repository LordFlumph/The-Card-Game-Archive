namespace CardGameArchive.Rules
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

    public class GolfGameRules : BaseGameRules
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

			return card == cardZoneParent.BottomCard;
		}

		public override List<Card> GetCardChain(Card card) => new List<Card>() { card };

		public override bool IsWinConditionAchieved() => GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau).All(o => o.CardCount == 0);

		protected override bool IsFoundationMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard)
		{
			if (mode == MoveValidationMode.Cheat)
			{
				StandardGameManager.Instance.CheatUsed();
				return true;
			}
				
			if (card.GetZoneParent().Zone == GameBoard.CardZone.Stock)
				return true;

			if (StandardGameManager.Instance.Variant == GameTerms.GameVariant.GolfTraditional)
			{
				// Can't place a card on top of a King in Standard Golf
				if (parentCard.Rank == Card.CardRank.King)
					return false;
			}
			else if (StandardGameManager.Instance.Variant == GameTerms.GameVariant.GolfPuttPutt)
			{
				if (card.Rank == Card.CardRank.King && parentCard.Rank == Card.CardRank.Ace)
					return true;
				if (card.Rank == Card.CardRank.Ace && parentCard.Rank == Card.CardRank.King)
					return true;
			}

			if (Mathf.Abs(GetRankValue(card) - GetRankValue(parentCard)) == 1)
				return true;

			return false;
		}
	}
}