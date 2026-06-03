namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "TableauFixedNumberDealSetupBehaviour", menuName = "Game Behaviour/Game Setup Behaviour/Tableau Fixed Number Deal")]
	public class TableauFixedNumberDealSetupBehaviour : BaseGameDealSetupBehaviour
	{
		[SerializeField] int cardsToDeal;
		[Tooltip("0 - no cards will be visible\n1 - only the last card dealt will be visible\n2 - the last two cards dealt will be visible\netc.")]
		[SerializeField] int faceUpCards;
		public override async Task DealCards()
		{
			List<ZoneParent> tableau = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau);
			Deck deck = GameBoard.Instance.GetDeck();
			
			if (deck == null || tableau.Count == 0)
			{
				Debug.LogWarning("Unable to deal cards");
				return;
			}

			int dealsRemaining = cardsToDeal;
			while (dealsRemaining > 0)
			{
				foreach (ZoneParent parent in tableau)
				{
					if (deck.RemainingCards <= 0)
					{
						Debug.LogWarning("Attempted to deal more cards than are in the deck");
						return;
					}

					Card card = deck.Draw();
					await DealCard(card, parent);

					if (dealsRemaining <= faceUpCards)
					{
						GameTaskManager.Instance.AddTask(card.SetFlipped(false));
					}

					dealsRemaining--;

					if (dealsRemaining <= 0)
						return;
				}
			}
		}
	}
}
