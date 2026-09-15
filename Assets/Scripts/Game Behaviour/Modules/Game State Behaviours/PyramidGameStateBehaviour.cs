namespace CardGameArchive.Behaviours
{
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using System;

	[CreateAssetMenu(fileName = "PyramidGameStateBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Game State Behaviours/Pyramid")]
	public class PyramidGameStateBehaviour : BaseGameStateBehaviour
	{
		public override bool IsGameStuck()
		{
			List<Card> tableauCards = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau).Where(o => o.BottomCard?.Interactable == true).Select(o => o.BottomCard).ToList();
			List<Card> cardsToCheck = new(tableauCards);

			if (StandardGameManager.Instance.Variant == GameTerms.GameVariant.PyramidTraditional)
			{
				// Game is stuck if all are true
				// 1. No cards remain in stock
				// 2. There are no possible pairs between all cards in Tableau and Waste

				if (GameBoard.Instance.GetDeck().Cards.Count != 0)
					return false;

				if (GameBoard.Instance.GetZoneParent(GameBoard.CardZone.Waste).BottomCard != null)
					cardsToCheck.Add(GameBoard.Instance.GetZoneParent(GameBoard.CardZone.Waste).BottomCard);
			}
			else
			{
				// Game is stuck if there are no pairs in the Tableau, or between the Tableau and ANY card in the Waste or Stock
				cardsToCheck.AddRange(GameBoard.Instance.GetZoneParent(GameBoard.CardZone.Stock).Cards.Select(o => o.Data));
				cardsToCheck.AddRange(GameBoard.Instance.GetZoneParent(GameBoard.CardZone.Waste).Cards.Select(o => o.Data));
			}


			// Check for any possible pairs for cards in the pyramid

			for (int i = 0; i < tableauCards.Count; i++)
			{
				if (tableauCards[i].Rank == Card.CardRank.King)
					return false;

				int rankValue = BaseGameRules.ActiveRules.GetRankValue(tableauCards[i].Rank);

				for (int j = 0; j < cardsToCheck.Count; j++)
				{
					if (tableauCards[i] == cardsToCheck[j])
						continue;

					if (rankValue + BaseGameRules.ActiveRules.GetRankValue(cardsToCheck[j].Rank) == 13)
						return false;
				}
			}

			return true;
		}
	}

}