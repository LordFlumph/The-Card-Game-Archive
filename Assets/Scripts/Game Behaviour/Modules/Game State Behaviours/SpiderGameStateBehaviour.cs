namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "SpiderGameStateBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Game State Behaviours/Spider")]
	public class SpiderGameStateBehaviour : BaseGameStateBehaviour
	{
		public override bool IsGameStuck()
		{
			if (GameBoard.Instance.GetDeck().RemainingCards > 0)
				return false;

			List<Card> cardsToCheck = new();

			foreach (Card card in GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau).Select(o => o.BottomCard))
			{
				if (card != null)
					cardsToCheck.AddRange(BaseGameRules.ActiveRules.GetCardChain(card));
				else // If there is an empty tableau space, then there is always a valid move
					return false;
			}

			foreach (Card card in cardsToCheck)
			{
				List<ZoneParent> possibleMoves = StandardGameManager.Instance.GetPossibleMoves(card);
				if (possibleMoves.Count > 0)
				{
					foreach (ZoneParent parent in possibleMoves)
					{
						// Check if moving would actually affect the game state
						if (card.linkedObj.TryGetParentCard(out CardObject parentCard))
						{
							Card newParentCard = parent.BottomCard;

							if (parentCard.Flipped == false)
								return false;

							// If we are moving to a different rank or suit, then we have a valid move
							if (newParentCard.Rank != parentCard.Rank || (newParentCard.Suit != parentCard.Suit && newParentCard.Suit == card.Suit))
							{
								return false;
							}
						}
						else // This card is at the bottom of the tableau, so moving it will free up space
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