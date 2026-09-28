namespace CardGameArchive.Rules
{
	using System.Collections.Generic;
	using System.Linq;
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
				if ((GetRankValue(activeCard.Rank) - GetRankValue(newCard.Rank)) == 1)
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

				if ((GetRankValue(activeCard.Rank) - GetRankValue(newCard.Rank)) == -1)
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

		public override bool IsWinConditionAchieved() => GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation).All(o => o.CardCount == 4);

		public override bool IsLossConditionAchieved()
		{
			if (IsWinConditionAchieved())
				return false;

			return GameBoard.Instance.GetZoneParent(GameBoard.CardZone.Foundation, 12).CardCount == 4;
		}
	}
}