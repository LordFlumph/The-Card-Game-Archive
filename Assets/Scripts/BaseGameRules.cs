namespace CardGameArchive
{
	using System.Collections.Generic;
	using UnityEngine;

    public abstract class BaseGameRules
    {
        public abstract bool CheckWinCondition();
        public virtual bool IsMoveValid(Card card, Transform newParent, Card parentCard = null)
        {
			if (card?.linkedObj == null || newParent == null)
				return false;

			if (card.linkedObj.transform.parent == null)
				return false;

			if (card.linkedObj.transform.parent == newParent)
				return true;

			ZoneIdentifier newZone = newParent.GetComponentInParent<ZoneIdentifier>();
			if (newZone == null)
			{
				Debug.LogWarning("Cannot move to a parent without a ZoneIdentifier component");
				return false;
			}

			ZoneIdentifier currentZone = card.linkedObj.GetComponentInParent<ZoneIdentifier>();
			if (currentZone == null)
			{
				Debug.LogWarning("Card does not have a parent with a ZoneIdentifier component");
				return false;
			}

			return newZone.Zone switch
            {
                GameBoard.CardZone.Stock => IsStockMoveValid(card, newParent, currentZone.Zone, parentCard),
                GameBoard.CardZone.Waste => IsWasteMoveValid(card, newParent, currentZone.Zone, parentCard),
				GameBoard.CardZone.Foundation => IsFoundationMoveValid(card, newParent, currentZone.Zone, parentCard),
                GameBoard.CardZone.Tableau => IsTableauMoveValid(card, newParent, currentZone.Zone, parentCard),
				_ => false,
            };
		}
		public abstract bool IsStockMoveValid(Card card, Transform newParent, GameBoard.CardZone currentZone, Card parentCard = null);
		public abstract bool IsWasteMoveValid(Card card, Transform newParent, GameBoard.CardZone currentZone, Card parentCard = null);
		public abstract bool IsFoundationMoveValid(Card card, Transform newParent, GameBoard.CardZone currentZone, Card parentCard = null);
		public abstract bool IsTableauMoveValid(Card card, Transform newParent, GameBoard.CardZone currentZone, Card parentCard = null);
        
		public virtual int GetScore() => 0;
        public abstract int GetRankValue(Card.CardRank rank);
    }
}