namespace CardGameArchive.Rules
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PyramidGameRules : BaseGameRules
    {
        public override bool CanCardMove(Card card)
		{
			// Raycast to the bottom corners of the card. If both hit the card then the answer is yes
			SpriteRenderer cardSR = card.linkedObj.GetComponent<SpriteRenderer>();
			
			RaycastHit2D rightCornerHit = Physics2D.Raycast(new Vector2(cardSR.bounds.max.x, cardSR.bounds.min.y), Vector3.forward);
			Debug.Log("Right corner hit: " + (rightCornerHit.collider.gameObject == cardSR.gameObject ? "Same card" : "Different card"));
			if (rightCornerHit.collider.gameObject != card.linkedObj.gameObject)
				return false;

			RaycastHit2D leftCornerHit = Physics2D.Raycast(new Vector2(cardSR.bounds.min.x, cardSR.bounds.min.y), Vector3.forward);
			Debug.Log("Left corner hit: " + (leftCornerHit.collider.gameObject == cardSR.gameObject ? "Same card" : "Different card"));
			if (leftCornerHit.collider.gameObject != card.linkedObj.gameObject)
				return false;

			return true;
		}

        public override List<Card> GetCardChain(Card card)
        {
			return new() { card };
        }

        public override int GetRankValue(Card.CardRank rank) => rank switch
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

        public override bool IsWinConditionAchieved()
		{
			if (StandardGameManager.Instance.Variant == GameTerms.GameVariant.PyramidTraditional)
			{
				return GameBoard.Instance.GetZoneParent(GameBoard.CardZone.Foundation, 0).CardCount == 52;
			}
				
			else if (StandardGameManager.Instance.Variant == GameTerms.GameVariant.PyramidRelaxed)
			{
				foreach (var tableau in GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau))
				{
					if (tableau.CardCount > 0)
						return false;
				}
				return true;
			}

			throw new System.NotImplementedException("Missing win condition for variant: " + StandardGameManager.Instance.Variant.ToString());
		}
	}
}
