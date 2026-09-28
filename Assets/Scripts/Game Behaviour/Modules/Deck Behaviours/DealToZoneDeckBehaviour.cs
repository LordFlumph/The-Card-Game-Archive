namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "DealToZoneDeckBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Deck Behaviours/Deal To Zone")]
	public class DealToZoneDeckBehaviour : BaseDeckBehaviour
	{
		[SerializeField] GameBoard.CardZone targetZone;
		[SerializeField] int targetZoneIndex;
		[SerializeField] int cardsToDeal = 1;

		protected override void OnDeckTapped(Deck deck)
		{
			if (deck.RemainingCards == 0)
				return;

			for (int i = 0; i < cardsToDeal; i++)
			{
				if (deck.RemainingCards == 0)
					break;

				bool forceContingent = deck.RemainingCards > 1 && i < cardsToDeal - 1;
				GameBoard.Instance.MoveCard(deck.Draw(), targetZone, targetZoneIndex, forceContingent: forceContingent);
			}
		}
	}

}