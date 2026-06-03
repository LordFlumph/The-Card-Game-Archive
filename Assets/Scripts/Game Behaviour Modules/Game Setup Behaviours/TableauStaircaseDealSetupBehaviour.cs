namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "TableauStaircaseDealSetupBehaviour", menuName = "Game Behaviour : ScriptableObjects/Game Setup Behaviour : ScriptableObjects/Tableau Staircase Deal")]
	public class TableauStaircaseDealSetupBehaviour : BaseGameSetupBehaviour
	{
		[Tooltip("The amount of cards to increase the deal by for each tableau")]
		[SerializeField] int stepAmount = 1;

		[Tooltip("The amount of cards to deal to the first tableau")]
		[SerializeField] int firstCardAmount = 1;

		[Tooltip("0 - no cards will be visible\n1 - only the last card will be visibile\n2 - the last two cards will be visible\netc.")]
		[SerializeField] int faceUpCardsPerTableau;

		public enum DealDirection
		{
			LeftRight,
			RightLeft,
		}
		[SerializeField] DealDirection direction;

		public override async Task DealCards()
		{
			List<ZoneParent> tableau = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau);
			Deck deck = GameBoard.Instance.GetDeck();

			if (deck == null || tableau.Count == 0)
			{
				Debug.LogWarning("Unable to deal cards");
				return;
			}

			if (direction == DealDirection.RightLeft)
				tableau.Reverse();

			int dealAmount = firstCardAmount;
			int rowsDealt = 0;
			for (int i = 0; i < tableau.Count; i++)
			{
				while (rowsDealt < dealAmount)
				{
					rowsDealt++;
					for (int j = i; j < tableau.Count; j++)
					{
						ZoneParent parent = tableau[j];
						if (deck.RemainingCards <= 0)
						{
							Debug.LogWarning("Attempted to deal more cards than are in the deck");
							return;
						}

						Card card = deck.Draw();
						await DealCard(card, parent);

						int targetCardsInParent = firstCardAmount + (j * stepAmount);
						bool flip = parent.CardCount > targetCardsInParent - faceUpCardsPerTableau;
						if (flip)
						{
							GameTaskManager.Instance.AddTask(card.SetFlipped(true));
						}
					}
				}
				dealAmount += stepAmount;
			}
		}
	}

}