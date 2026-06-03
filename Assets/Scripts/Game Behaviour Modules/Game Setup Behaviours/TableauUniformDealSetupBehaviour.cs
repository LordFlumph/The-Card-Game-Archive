namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "TableauUniformDealSetupBehaviour", menuName = "Game Behaviour/Game Setup Behaviour/Tableau Uniform Deal")]
	public class TableauUniformDealSetupBehaviour : BaseGameDealSetupBehaviour
	{
		[SerializeField] int cardsPerTableau;
		[Tooltip("0 - no cards will be visible\n1 - only the last card will be visibile\n2 - the last two cards will be visible\netc.")]
		[SerializeField] int faceUpCardsPerTableau;
		public override async Task DealCards()
		{
			List<ZoneParent> tableau = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau);
			Deck deck = GameBoard.Instance.GetDeck();
			
			if (deck == null || tableau.Count == 0)
			{
				Debug.LogWarning("Unable to deal cards");
				return;
			}

			// Breaking convention and going from 1->max instead of 0->max-1 because it simplifies the logic for my brain
			for (int i = 1; i <= cardsPerTableau; i++)
			{
				foreach (ZoneParent parent in tableau)
				{
					if (deck.RemainingCards <= 0)
					{
						Debug.LogWarning("Attempted to deal more cards than are in the deck");
						return;
					}

					Card card = deck.Draw();
					if (i > cardsPerTableau - faceUpCardsPerTableau)
					{
						GameTaskManager.Instance.AddTask(card.SetFlipped(true));
					}
					await DealCard(card, parent);
				}
			}
		}
	}
}
