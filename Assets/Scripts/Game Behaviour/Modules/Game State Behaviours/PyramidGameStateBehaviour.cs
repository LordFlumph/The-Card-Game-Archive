namespace CardGameArchive.Behaviours
{
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "PyramidGameStateBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Game State Behaviours/Pyramid")]
	public class PyramidGameStateBehaviour : BaseGameStateBehaviour
	{
		public override bool IsGameStuck()
		{
			// Game is stuck if the only valid pair for a card is being blocked by this card
			List<Card> tableauCards = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau).Where(o => o.BottomCard != null && o.BottomCard.Interactable).Select(o => o.BottomCard).ToList();
			foreach (Card card in tableauCards)
			{
				
			}

			if (StandardGameManager.Instance.Variant == GameTerms.GameVariant.PyramidTraditional)
			{
				// Game is stuck if all are true
				// 1. No cards remain in stock
				// 2. There are no possible pairs between all cards in Tableau and Waste
			}
			else
			{
				// Game is stuck if there are no pairs in the Tableau, or between the Tableau and ANY card in the Waste or Stock
			}

			return false;
		}
	}

}