namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "PyramidDeckVerificationBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Deck Verification Behaviours/Pyramid")]
    public class PyramidDeckVerificationBehaviour : BaseDeckVerificationBehaviour
    {
		protected override bool VerifyDeck()
		{
            // Break deck into actual rows
            List<Card> allCards = GameBoard.Instance.GetDeck().Cards;

            List<List<Card>> rows = new List<List<Card>>();
			rows.Add(new() { allCards[^1] });
			rows.Add(new() { allCards[^2], allCards[^3] });
			rows.Add(new() { allCards[^4], allCards[^5], allCards[^6] });
			rows.Add(new() { allCards[^7], allCards[^8], allCards[^9], allCards[^10] });
			rows.Add(new() { allCards[^11], allCards[^12], allCards[^13], allCards[^14], allCards[^15] });
			rows.Add(new() { allCards[^16], allCards[^17], allCards[^18], allCards[^19], allCards[^20], allCards[^21] });
			rows.Add(new() { allCards[^22], allCards[^23], allCards[^24], allCards[^25], allCards[^26], allCards[^27], allCards[^28] });


			// Ensure that there are no duplicate ranks in rows 1-3
			{
				List<Card.CardRank> first3Rows = new();
				first3Rows.AddRange(rows[0].Select(o => o.Rank));
				first3Rows.AddRange(rows[1].Select(o => o.Rank));
				first3Rows.AddRange(rows[2].Select(o => o.Rank));
				if (first3Rows.Distinct().Count() != first3Rows.Count)
					return false;
			}


			// Ensure that there is no more than one duplicate in the first row
			{
				if (rows[^1].Distinct().Count() < rows[^1].Count - 1)
					return false;
			}


			// Ensure there is at least one move in the first row
			{
				bool validMove = false;
				for (int i = 0; i < rows[^1].Count-1; i++)
				{
					for (int j = i+1; j < rows[^1].Count; j++)
					{
						if (BaseGameRules.ActiveRules.GetRankValue(rows[^1][i].Rank) + BaseGameRules.ActiveRules.GetRankValue(rows[^1][j].Rank) == 13)
						{
							validMove = true;
							break;
						}
					}

					if (validMove)
						break;
				}

				if (!validMove)
					return false;
			}

			return true;
		}
    }

}