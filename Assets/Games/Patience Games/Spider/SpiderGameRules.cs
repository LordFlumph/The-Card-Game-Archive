namespace CardGameArchive.Rules
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

		protected override bool IsTableauMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard)
		{
			if (mode == MoveValidationMode.Cheat)
			{
				StandardGameManager.Instance.CheatUsed();
				return true;
			}
				

			if (destination.CardCount == 0)
				return true;

			if (GetRankValue(parentCard.Rank) - GetRankValue(card.Rank) == 1)
				return true;

			return false;
		}
		protected override bool IsStockMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard) => false;
		protected override bool IsFoundationMoveValid(Card card, ZoneParent destination, Card parentCard = null, MoveValidationMode mode = MoveValidationMode.Standard) => false;

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
				if ((GetRankValue(activeCard.Rank) - GetRankValue(newCard.Rank)) == 1
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

				if ((GetRankValue(activeCard.Rank) - GetRankValue(newCard.Rank)) == -1
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