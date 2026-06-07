namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "KlondikeDeckVerificationBehaviour", menuName = "Card Game Archive/Game Behaviour/Deck Verification Behaviours/Klondike")]
	public class KlondikeDeckVerificationBehaviour : BaseDeckVerificationBehaviour
	{
		protected override bool VerifyDeck()
		{
			List<Card> cards = new List<Card>();
			List<List<Card>> tableau = new() { new(), new(), new(), new(), new(), new(), new() };
			Deck deck = GameBoard.Instance.GetDeck();

			while (deck.RemainingCards > 0)
			{
				cards.Add(deck.Draw());
			}

			for (int i = cards.Count - 1; i >= 0; i--)
			{
				deck.AddCard(cards[i]);
			}

			for (int i = 0; i < 7; i++)
			{
				for (int j = i; j < 7; j++)
				{
					Card card = cards[0];
					cards.RemoveAt(0);

					tableau[j].Add(card);
				}
			}

			// Fail if there are no immediate moves for the Tableau
			List<Card> lastCards = tableau.Select(o => o[^1]).ToList();
			bool validMoves = false;
			for (int i = 0; i < lastCards.Count; i++)
			{
				// An Ace will instantly go to the Foundation, so it isn't considered a player move for this verification
				if (lastCards[i].Rank == Card.CardRank.Ace)
					continue;

				for (int j = i + 1; j < lastCards.Count; j++)
				{
					if (Mathf.Abs(StandardGameManager.Instance.Rules.GetRankValue(lastCards[i]) - StandardGameManager.Instance.Rules.GetRankValue(lastCards[j])) == 1 &&
						Card.SuitColors[lastCards[i].Suit] != Card.SuitColors[lastCards[j].Suit])
					{
						validMoves = true;
						break;
					}
				}

				if (validMoves)
				{
					break;
				}
			}

			if (!validMoves)
			{
				return false;
			}


			// Fail if any aces are buried deep in the tableau
			foreach (List<Card> tableauCards in tableau)
			{
				List<Card> aces = tableauCards.Where(o => o.Rank == Card.CardRank.Ace).ToList();
				if (aces.Count > 0)
				{
					foreach (Card ace in aces)
					{
						if (tableauCards.Count - tableauCards.IndexOf(ace) >= 3)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
	}
}