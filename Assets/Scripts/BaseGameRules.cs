namespace CardGameArchive
{
	using System.Collections.Generic;
	using UnityEngine;

    public abstract class BaseGameRules
    {
        public abstract bool CheckWinCondition();
        public virtual bool IsMoveValid(Card card, ZoneParent zoneParent)
        {
			if (!CanCardMove(card))
				return false;

			// Card wouldn't be moving
			if (card.linkedObj.GetZoneParent() == zoneParent)
				return false;

			Card parentCard = null;
			if (zoneParent.transform.childCount > 0)
			{
				parentCard = zoneParent.transform.GetBottomChild().GetComponent<CardObject>().CardData;
			}

			if (card.GetZoneParent() == null)
			{
				Debug.LogWarning("Card does not have a parent with a ZoneIdentifier component");
				return false;
			}

			return zoneParent.Zone switch
            {
                GameBoard.CardZone.Stock => IsStockMoveValid(card, zoneParent, parentCard),
                GameBoard.CardZone.Waste => IsWasteMoveValid(card, zoneParent, parentCard),
				GameBoard.CardZone.Foundation => IsFoundationMoveValid(card, zoneParent, parentCard),
                GameBoard.CardZone.Tableau => IsTableauMoveValid(card, zoneParent, parentCard),
				_ => false,
            };
		}

		public abstract bool CanCardMove(Card card);
		protected abstract bool IsStockMoveValid(Card card, ZoneParent zoneParent, Card parentCard = null);
		protected abstract bool IsWasteMoveValid(Card card, ZoneParent zoneParent, Card parentCard = null);
		protected abstract bool IsFoundationMoveValid(Card card, ZoneParent zoneParent, Card parentCard = null);
		protected abstract bool IsTableauMoveValid(Card card, ZoneParent zoneParent, Card parentCard = null);
        
		public virtual int GetScore() => 0;
        public abstract int GetRankValue(Card.CardRank rank);
    }
}