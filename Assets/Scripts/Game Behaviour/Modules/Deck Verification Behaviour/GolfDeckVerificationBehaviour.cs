namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "GolfDeckVerificationBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Deck Verification Behaviours/Golf")]
	public class GolfDeckVerificationBehaviour : BaseDeckVerificationBehaviour
	{
		protected override bool VerifyDeck()
		{
			Deck deck = GameBoard.Instance.GetDeck();
			List<Card> cards = deck.Cards;

			List<List<Card>> tableau = new();
			for (int row = 0; row < 5; row++)
			{
				List<Card> tableauRow = new();
				for (int column = 0; column < 7; column++)
				{
					int cardIndexFromEnd = (row * 7) + column + 1;
					tableauRow.Add(cards[^cardIndexFromEnd]);
				}

				tableau.Add(tableauRow);
			}
			List<Card> deckCards = cards.GetRange(0, cards.Count-35);
			deckCards.Reverse();


			// Ensure that there aren't more than 2 kings covering queens
			{
				int kingsOnQueens = 0;
				for (int i = 0; i < tableau.Count; i++)
				{
					for (int j = 1; j < 7; j++)
					{
						if (tableau[i][j].Rank == Card.CardRank.King && tableau[i][j - 1].Rank == Card.CardRank.Queen)
						{
							kingsOnQueens++;
						}
					}
				}

				if (kingsOnQueens > 2)
					return false;
			}

			// Ensure that there aren't more than 4 of the same rank adjacent to each other in the deck
			{
				int adjacentSameRank = 0;

				for (int i = 1; i < deckCards.Count - 1; i++)
				{
					if (deckCards[i].Rank == deckCards[i - 1].Rank)
					{
						adjacentSameRank++;
					}
				}

				if (adjacentSameRank > 4)
					return false;
			}

			return true;
		}
	}

}