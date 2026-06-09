namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "FixedNumberDealSetupBehaviour", menuName = "Card Game Archive/Game Behaviour/Game Deal Behaviour/Fixed Number Deal")]
	public class FixedNumberDealSetupBehaviour : BaseGameDealBehaviour
	{
		[SerializeField] GameBoard.CardZone dealZone = GameBoard.CardZone.Tableau;

		[SerializeField] int cardsToDeal;
		[Tooltip("0 - no cards will be visible\n1 - only the last card dealt will be visible\n2 - the last two cards dealt will be visible\netc.")]
		[SerializeField] int faceUpCards;
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

			int dealsRemaining = cardsToDeal;
			while (dealsRemaining > 0)
			{
				foreach (ZoneParent parent in dealZoneParent)
				{
					if (deck.RemainingCards <= 0)
					{
						Debug.LogWarning("Attempted to deal more cards than are in the deck");
						return;
					}

					Card card = deck.Draw();

					if (dealsRemaining <= faceUpCards)
					{
						GameTaskManager.Instance.AddTask(card.SetFlipped(true));
						card.SetInteractable(true);

						if (recordMoves)
						{
							bool firstFlip = dealsRemaining == faceUpCards;
							StandardGameManager.Instance.MoveTaken(new GameMove(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(card, true, !firstFlip)));
						}
					}

					await DealCard(card, parent);

					dealsRemaining--;

					if (dealsRemaining <= 0)
						return;
				}
			}
		}
	}
}
