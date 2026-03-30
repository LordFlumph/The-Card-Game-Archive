using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardGameArchive.Solitaire.Klondike
{
	public class KlondikeGameRules : BaseGameRules
	{
		public override bool IsWinConditionAchieved()
		{
			// Simple, if all 4 foundations have 13 cards in them, then we win
			List<ZoneParent> foundationParents = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
			return foundationParents.Where(o => o.CardCount == 13).Count() == 4;
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

		public override bool CanCardMove(Card card)
		{
			if (card?.linkedObj == null)
				return false;

			if (!card.Flipped)
				return false;

			ZoneParent cardZoneParent = card.GetZoneParent();

			if (cardZoneParent == null)
				return false;

			if (cardZoneParent.ZoneFull)
				return false;

			switch (cardZoneParent.Zone)
			{
				case GameBoard.CardZone.Tableau:
					// Card can only move if it is part of the last card's chain
					return GameBoard.Instance.GetCardChain(card.linkedObj)[^1] == cardZoneParent.BottomCard;
				default:
					return card == cardZoneParent.BottomCard;
			}
		}

		protected override bool IsStockMoveValid(Card card, ZoneParent zoneParent, Card parentCard) => card.GetZoneParent().Zone == GameBoard.CardZone.Waste;
		protected override bool IsWasteMoveValid(Card card, ZoneParent zoneParent, Card parentCard) => card.GetZoneParent().Zone == GameBoard.CardZone.Stock;

		protected override bool IsFoundationMoveValid(Card card, ZoneParent zoneParent, Card parentCard)
		{
			if (!(card.GetZoneParent().Zone is GameBoard.CardZone.Tableau or GameBoard.CardZone.Waste))
				return false;

			// We can't move a stack of cards into the foundation
			if (card.linkedObj.transform.childCount != 0)
				return false;

			// If we have a parent card, just check if we can be placed on it
			if (parentCard != null)
			{
				if (parentCard.Data.suit != card.Data.suit)
					return false;

				if (GetRankValue(card.Rank) - GetRankValue(parentCard.Rank) != 1)
					return false;

				return true;
			}
			// If we don't have a parent card, then this foundation is empty
			else
			{
				if (card.Data.rank != Card.CardRank.Ace)
					return false;

				List<ZoneParent> foundationParents = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
				Transform suitParent = zoneParent.transform;
				foreach (var parent in foundationParents)
				{
					if (parent.transform.childCount == 0)
						continue;

					CardObject childCard = parent.transform.GetBottomChild().GetComponent<CardObject>();
					if (childCard != null && childCard.Suit == card.Suit)
					{
						suitParent = parent.transform;
						break;
					}
				}

				if (suitParent == zoneParent.transform)
					return true;

				return false;
			}
		}

		protected override bool IsTableauMoveValid(Card card, ZoneParent zoneParent, Card parentCard)
		{
			if (card.GetZoneParent().Zone == GameBoard.CardZone.Stock)
				return false;

			// If there is no parent card then the zone is empty, so only a king can be placed here
			if (parentCard == null)
				return card.Rank == Card.CardRank.King;

			if (Card.SuitColours[card.Suit] == Card.SuitColours[parentCard.Suit])
				return false;

			if (GetRankValue(parentCard.Rank) - GetRankValue(card.Rank) != 1)
				return false;

			return true;
		}
	}
}
