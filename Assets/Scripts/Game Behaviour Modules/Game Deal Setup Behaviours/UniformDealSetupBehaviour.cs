namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "UniformDealSetupBehaviour", menuName = "Card Game Archive/Game Behaviour/Game Deal Behaviour/Uniform Deal")]
	public class UniformDealSetupBehaviour : BaseGameDealBehaviour
	{
		[SerializeField] GameBoard.CardZone dealZone = GameBoard.CardZone.Tableau;

		[SerializeField] int cardsPerZone;
		[Tooltip("0 - no cards will be visible\n1 - only the last card will be visibile\n2 - the last two cards will be visible\netc.")]
		[SerializeField] int faceUpCardsPerZone;
		public override async Task DealCards()
		{
			List<ZoneParent> dealZoneParent = GameBoard.Instance.GetZoneParents(dealZone);
			Deck deck = GameBoard.Instance.GetDeck();
			
			if (deck == null || dealZoneParent.Count == 0)
			{
				Debug.LogWarning("Unable to deal cards");
				return;
			}

			// Breaking convention and going from 1->max instead of 0->max-1 because it simplifies the logic for my brain
			for (int i = 1; i <= cardsPerZone; i++)
			{
				foreach (ZoneParent parent in dealZoneParent)
				{
					if (deck.RemainingCards <= 0)
					{
						Debug.LogWarning("Attempted to deal more cards than are in the deck");
						return;
					}

					Card card = deck.Draw();
					if (i > cardsPerZone - faceUpCardsPerZone)
					{
						GameTaskManager.Instance.AddTask(card.SetFlipped(true));
					}
					await DealCard(card, parent);
				}
			}
		}
	}
}
