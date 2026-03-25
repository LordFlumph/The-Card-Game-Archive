namespace CardGameArchive
{
	using System.Collections.Generic;
	using UnityEngine;

    public abstract class BaseGameRules
    {
        public abstract bool CheckWinCondition();
        public virtual bool IsMoveValid(Card card, ZoneIdentifier zoneParent, Card parentCard = null)
        {
			if (card?.linkedObj == null || zoneParent == null)
				return false;

			if (card.linkedObj.transform.parent == null)
				return false;

			if (card.linkedObj.transform.parent == zoneParent)
				return true;

			// Confirm if there is a parent card
			if (parentCard == null)
			{
				if (zoneParent.transform.childCount > 0)
				{
					parentCard = zoneParent.transform.GetBottomChild().GetComponent<CardObject>().cardData;
				}
			}


			ZoneIdentifier currentZone = card.linkedObj.GetComponentInParent<ZoneIdentifier>();
			if (currentZone == null)
			{
				Debug.LogWarning("Card does not have a parent with a ZoneIdentifier component");
				return false;
			}

			return zoneParent.Zone switch
            {
                GameBoard.CardZone.Stock => IsStockMoveValid(card, zoneParent, currentZone.Zone, parentCard),
                GameBoard.CardZone.Waste => IsWasteMoveValid(card, zoneParent, currentZone.Zone, parentCard),
				GameBoard.CardZone.Foundation => IsFoundationMoveValid(card, zoneParent, currentZone.Zone, parentCard),
                GameBoard.CardZone.Tableau => IsTableauMoveValid(card, zoneParent, currentZone.Zone, parentCard),
				_ => false,
            };
		}
		public abstract bool IsStockMoveValid(Card card, ZoneIdentifier zoneParent, GameBoard.CardZone currentZone, Card parentCard = null);
		public abstract bool IsWasteMoveValid(Card card, ZoneIdentifier zoneParent, GameBoard.CardZone currentZone, Card parentCard = null);
		public abstract bool IsFoundationMoveValid(Card card, ZoneIdentifier zoneParent, GameBoard.CardZone currentZone, Card parentCard = null);
		public abstract bool IsTableauMoveValid(Card card, ZoneIdentifier zoneParent, GameBoard.CardZone currentZone, Card parentCard = null);
        
		public virtual int GetScore() => 0;
        public abstract int GetRankValue(Card.CardRank rank);
    }
}