namespace CardGameArchive.Rules
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PyramidGameRules : BaseGameRules
    {
        public override bool CanCardMove(Card card) => false;

        public override List<Card> GetCardChain(Card card)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
    }
}
