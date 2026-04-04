namespace CardGameArchive
{
	using UnityEngine;

	/// <summary>
	/// Base class defining the rules of a card game
	/// Handles whether or not a move is valid, what it takes to win, etc.
	/// </summary>
    public abstract class BaseGameRules
    {
        public abstract bool IsWinConditionAchieved();
        public virtual bool IsMoveValid(Card card, ZoneParent destination)
        {
			if (!CanCardMove(card))
				return false;

			// Card wouldn't be moving
			if (card.linkedObj.GetZoneParent() == destination)
				return false;

			Card parentCard = null;
			if (destination.transform.childCount > 0)
			{
				parentCard = destination.transform.GetBottomChild().GetComponent<CardObject>().Data;
			}

			if (card.GetZoneParent() == null)
			{
				Debug.LogWarning("Card does not have a parent with a ZoneIdentifier component");
				return false;
			}

			return destination.Zone switch
            {
                GameBoard.CardZone.Stock => IsStockMoveValid(card, destination, parentCard),
                GameBoard.CardZone.Waste => IsWasteMoveValid(card, destination, parentCard),
				GameBoard.CardZone.Foundation => IsFoundationMoveValid(card, destination, parentCard),
                GameBoard.CardZone.Tableau => IsTableauMoveValid(card, destination, parentCard),
				_ => false,
            };
		}

		public abstract bool CanCardMove(Card card);
		protected abstract bool IsStockMoveValid(Card card, ZoneParent destination, Card parentCard = null);
		protected abstract bool IsWasteMoveValid(Card card, ZoneParent destination, Card parentCard = null);
		protected abstract bool IsFoundationMoveValid(Card card, ZoneParent destination, Card parentCard = null);
		protected abstract bool IsTableauMoveValid(Card card, ZoneParent destination, Card parentCard = null);
        
		public virtual int GetScore() => 0;
		public virtual int GetRankValue(Card card) => GetRankValue(card.Rank);
		public abstract int GetRankValue(Card.CardRank rank);
    }
}