namespace CardGameArchive.Solitaire.Spider
{
	using System.Collections.Generic;

	public class SpiderGameRules : BaseGameRules
    {
		public override bool CanCardMove(Card card)
		{
			if (card?.linkedObj == null)
				return false;

			if (!card.Flipped)
				return false;

			if (!card.Interactable)
				return false;

			ZoneParent cardZoneParent = card.GetZoneParent();

			if (cardZoneParent == null)
				return false;

			return GetCardChain(card)[^1] == cardZoneParent.BottomCard;
		}

		public override int GetRankValue(Card.CardRank rank)
		{
			return rank switch
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
		}

		public override bool IsWinConditionAchieved()
		{
			List<ZoneParent> parents = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
			foreach (var parent in parents)
			{
				if (parent.CardCount != 13)
					return false;
			}

			return true;
		}

		protected override bool IsTableauMoveValid(Card card, ZoneParent destination, Card parentCard = null, bool simulation = false)
		{
			throw new System.NotImplementedException();
		}

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
						BaseGameManager.Instance.Rules.GetRankValue(newCard.Rank)) == 1
						&& activeCard.Suit == newCard.Suit)
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
					BaseGameManager.Instance.Rules.GetRankValue(newCard.Rank)) == -1
					&& activeCard.Suit == newCard.Suit)
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
	}

}