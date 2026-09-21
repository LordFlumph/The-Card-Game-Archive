namespace CardGameArchive
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Base class defining the rules of a card game
	/// Handles whether or not a move is valid, what it takes to win, etc.
	/// </summary>
    public abstract class BaseGameRules
    {
		public static BaseGameRules ActiveRules { get { return StandardGameManager.Instance?.Rules; } }

        public abstract bool IsWinConditionAchieved();
		public virtual bool IsLossConditionAchieved() => false;

		public enum MoveValidationMode
		{
			Standard,
			Analysis,
			Cheat
		}
        public virtual bool IsMoveValid(Card card, ZoneParent destination, MoveValidationMode mode = MoveValidationMode.Standard)
        {
			if (mode != MoveValidationMode.Analysis && !CanCardMove(card))
				return false;

			// Card wouldn't be moving
			if (card.linkedObj.GetZoneParent() == destination)
				return false;

			Card parentCard = null;
			if (destination.CardCount > 0)
			{
				parentCard = destination.BottomCard;
			}

			if (card.GetZoneParent() == null)
			{
				Debug.LogWarning("Card does not have a parent with a ZoneParent component");
				return false;
			}

			if (mode == MoveValidationMode.Standard && StandardGameManager.Instance.CheatActive)
				mode = MoveValidationMode.Cheat;

			return destination.Zone switch
            {
                GameBoard.CardZone.Stock => IsStockMoveValid(card, destination, parentCard, mode),
                GameBoard.CardZone.Waste => IsWasteMoveValid(card, destination, parentCard, mode),
				GameBoard.CardZone.Foundation => IsFoundationMoveValid(card, destination, parentCard, mode),
                GameBoard.CardZone.Tableau => IsTableauMoveValid(card, destination, parentCard, mode),
				GameBoard.CardZone.Pile => IsPileMoveValid(card, destination, parentCard, mode),
				GameBoard.CardZone.Cell => IsCellMoveValid(card, destination, parentCard, mode),
				GameBoard.CardZone.Discard => IsDiscardMoveValid(card, destination, parentCard, mode),
				_ => false,
            };
		}
		public virtual bool IsMoveValid(Card card, Card destination, MoveValidationMode mode = MoveValidationMode.Standard)
		{
			if (mode != MoveValidationMode.Analysis && !CanCardMove(card))
				return false;

			// Can't move to the same card
			if (card == destination)
				return false;

			ZoneParent destinationParent = destination.GetZoneParent();

			if (destinationParent == null)
			{
				Debug.LogWarning("Card does not have a parent with a ZoneParent component");
				return false;
			}

			return destinationParent.Zone switch
			{
				GameBoard.CardZone.Stock => IsStockMoveValid(card, destinationParent, destination, mode),
				GameBoard.CardZone.Waste => IsWasteMoveValid(card, destinationParent, destination, mode),
				GameBoard.CardZone.Foundation => IsFoundationMoveValid(card, destinationParent, destination, mode),
				GameBoard.CardZone.Tableau => IsTableauMoveValid(card, destinationParent, destination, mode),
				GameBoard.CardZone.Pile => IsPileMoveValid(card, destinationParent, destination, mode),
				GameBoard.CardZone.Cell => IsCellMoveValid(card, destinationParent, destination, mode),
				GameBoard.CardZone.Discard => IsDiscardMoveValid(card, destinationParent, destination, mode),
				_ => false,
			};
		}
		public abstract bool CanCardMove(Card card);
		protected virtual bool IsStockMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard) => false;
		protected virtual bool IsWasteMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard) => false;
		protected virtual bool IsFoundationMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard) => false;
		protected virtual bool IsTableauMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard) => false;
		protected virtual bool IsPileMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard) => false;
		protected virtual bool IsCellMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard) => false;
		protected virtual bool IsDiscardMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard) => false;

		public virtual int GetRankValue(CardObject card) => GetRankValue(card.Rank);
		public virtual int GetRankValue(Card card) => GetRankValue(card.Rank);
		public abstract int GetRankValue(Card.CardRank rank);
		public virtual List<Card> GetCardChain(ZoneParent zone)
		{
			if (zone.CardCount > 0)
			{
				return GetCardChain(zone.BottomCard);
			}

			return new();
		}
		public abstract List<Card> GetCardChain(Card card);
    }
}