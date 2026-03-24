using System.Collections.Generic;
using UnityEngine;

namespace CardGameArchive.Solitaire.Klondike
{
	public class KlondikeGameRules : BaseGameRules
	{
		public override bool CheckWinCondition()
		{
			throw new System.NotImplementedException();
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

		public override bool IsStockMoveValid(Card card, Transform newParent, GameBoard.CardZone currentZone, Card parentCard = null) => currentZone == GameBoard.CardZone.Waste;
		public override bool IsWasteMoveValid(Card card, Transform newParent, GameBoard.CardZone currentZone, Card parentCard = null) => currentZone == GameBoard.CardZone.Stock;

		public override bool IsFoundationMoveValid(Card card, Transform newParent, GameBoard.CardZone currentZone, Card parentCard = null)
		{
			if (!(currentZone is GameBoard.CardZone.Tableau or GameBoard.CardZone.Waste))
				return false;

			// We can't move a stack of cards into the foundation
			if (card.linkedObj.transform.childCount != 0)
				return false;

			// Confirm if there is a parent card
			if (parentCard == null)
			{
				if (newParent.childCount > 0)
				{
					parentCard = newParent.GetChild(newParent.childCount - 1).GetComponent<CardObject>().cardData;
				}
			}

			// If we do have a parent card, just check if we can be placed on it
			if (parentCard != null)
			{
				if (parentCard.Data.suit != card.Data.suit)
					return false;

				if (GetRankValue(card.Data.rank) - GetRankValue(parentCard.Data.rank) != 1)
					return false;

				return true;
			}
			// If we don't have a parent card, then this foundation is empty
			else
			{
				if (card.Data.rank != Card.CardRank.Ace)
					return false;

				List<Transform> foundationParents = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
				Transform suitParent = newParent;
				foreach (var parent in foundationParents)
				{
					if (parent.childCount == 0)
						continue;

					CardObject childCard = parent.GetComponentInChildren<CardObject>();
					if (childCard != null && childCard.Data.suit == card.Data.suit)
					{
						suitParent = parent;
						break;
					}
				}

				if (suitParent == newParent)
					return true;

				return false;
			}
		}

		public override bool IsTableauMoveValid(Card card, Transform newParent, GameBoard.CardZone currentZone, Card parentCard = null)
		{
			if (currentZone == GameBoard.CardZone.Stock)
				return false;

			return true;
		}		
	}
}
