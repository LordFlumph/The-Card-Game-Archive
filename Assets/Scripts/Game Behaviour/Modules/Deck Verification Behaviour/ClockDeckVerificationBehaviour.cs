namespace CardGameArchive.Behaviours
{
    using UnityEngine;

	[CreateAssetMenu(fileName = "ClockDeckVerificationBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Deck Verification Behaviours/Clock")]
	public class ClockDeckVerificationBehaviour : BaseDeckVerificationBehaviour
	{
		protected override bool VerifyDeck()
		{
			Deck deck = GameBoard.Instance.GetDeck();
			int kingsInFirstDeal = 0;
			for (int i = 1; i < 14; i++)
			{
				if (deck.Cards[^i].Rank == Card.CardRank.King)
					kingsInFirstDeal++;

				if (kingsInFirstDeal > 1)
					return false;
			}

			return true;
		}
	}

}