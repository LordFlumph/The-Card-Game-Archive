namespace CardGameArchive.Rules
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PyramidGameRules : BaseGameRules
    {
        public override bool CanCardMove(Card card) => card.Interactable;

		public override List<Card> GetCardChain(Card card)
        {
			return new() { card };
        }

        public override bool IsWinConditionAchieved()
		{
			foreach (var tableau in GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau))
			{
				if (tableau.CardCount > 0)
					return false;
			}
			return true;
		}
	}
}
