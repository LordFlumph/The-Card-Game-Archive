namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "StaircaseDealSetupBehaviour", menuName = "Card Game Archive/Game Behaviour/Game Deal Behaviour/Staircase Deal")]
	public class StaircaseDealSetupBehaviour : BaseGameDealBehaviour
	{
		[SerializeField] GameBoard.CardZone dealZone = GameBoard.CardZone.Tableau;

		[Tooltip("The amount of cards to increase the deal by for each zone")]
		[SerializeField] int stepAmount = 1;

		[Tooltip("The amount of cards to deal to the first zone")]
		[SerializeField] int firstCardAmount = 1;

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

			if (direction == GameTerms.DealDirection.RightLeft)
				dealZoneParent.Reverse();

			int dealAmount = firstCardAmount;
			int rowsDealt = 0;
			for (int i = 0; i < dealZoneParent.Count; i++)
			{
				while (rowsDealt < dealAmount)
				{
					rowsDealt++;
					for (int j = i; j < dealZoneParent.Count; j++)
					{
						ZoneParent parent = dealZoneParent[j];
						if (deck.RemainingCards <= 0)
						{
							Debug.LogWarning("Attempted to deal more cards than are in the deck");
							return;
						}

						Card card = deck.Draw();

						int targetCardsInParent = firstCardAmount + (j * stepAmount);
						bool flip = parent.CardCount > targetCardsInParent - faceUpCardsPerZone;
						if (flip)
						{
							GameTaskManager.Instance.AddTask(card.SetFlipped(true));

							if (recordMoves)
							{
								bool firstFlip = i == 0 && parent.CardCount == targetCardsInParent - faceUpCardsPerZone + 1;
								StandardGameManager.Instance.MoveTaken(new GameMove(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(card, true, !firstFlip)));
							}
						}

						await DealCard(card, parent);
					}
				}
				dealAmount += stepAmount;
			}
		}
	}

}