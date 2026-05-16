namespace CardGameArchive.Solitaire.Clock
{
	using System.Collections.Generic;
	using UnityEngine;

	public class ClockGameRules : BaseGameRules
	{
		public override bool CanCardMove(Card card) => true;

		public override List<Card> GetCardChain(Card card)
		{
			if (card?.Data == null)
			{
				return new();
			}
			ZoneParent zoneParent = card.GetZoneParent();
			CardObject activeCard = card.linkedObj;

			while (activeCard.TryGetChildCard(out CardObject newCard))
			{
				if ((BaseGameManager.Instance.Rules.GetRankValue(activeCard.Rank) -
						BaseGameManager.Instance.Rules.GetRankValue(newCard.Rank)) == 1)
				{
					activeCard = newCard;
				}
				else
				{
					break;
				}
			}

			List<Card> cardChain = new();
			cardChain.Add(activeCard.Data);

			while (activeCard.TryGetParentCard(out CardObject newCard))
			{
				if (!newCard.Flipped)
				{
					break;
				}

				if ((BaseGameManager.Instance.Rules.GetRankValue(activeCard.Rank) -
					BaseGameManager.Instance.Rules.GetRankValue(newCard.Rank)) == -1)
				{
					cardChain.Add(newCard.Data);
					activeCard = newCard;
				}
				else
				{
					break;
				}
			}

			// Finally, reverse the card chain (since we want it to be from the first card in the chain down
			cardChain.Reverse();
			return cardChain;
		}

		public override int GetRankValue(Card.CardRank rank) => rank switch
		{
			Card.CardRank.Ace => 1,
			Card.CardRank.Two => 2,
			Card.CardRank.Three => 3,
			Card.CardRank.Four => 4,
			Card.CardRank.Five => 5,
			Card.CardRank.Six => 6,
			Card.CardRank.Seven => 7,
			Card.CardRank.Eight => 8,
			Card.CardRank.Nine => 9,
			Card.CardRank.Ten => 10,
			Card.CardRank.Jack => 11,
			Card.CardRank.Queen => 12,
			Card.CardRank.King => 13,
			_ => throw new System.ArgumentOutOfRangeException("Unexpected rank value")
		};

		public override bool IsWinConditionAchieved()
		{
			List<ZoneParent> foundations = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
			foreach (ZoneParent parent in foundations)
			{
				if (parent.CardCount != 4)
				{
					return false;
				}
			}

			return true;
		}

		public override bool IsLossConditionAchieved()
		{
			int faceUpKings = 0;
			bool gameFinished = true;
			foreach (Card card in GameBoard.Instance.AllCards)
			{
				if (!card.Flipped)
					gameFinished = false;

				if (card.Rank == Card.CardRank.King)
				{
					if (card.Flipped)
						faceUpKings++;
					else
						return false;
				}
			}

			if (gameFinished)
				return true;

			if (faceUpKings != 4)
				return false;

			return true;
		}
	}

}