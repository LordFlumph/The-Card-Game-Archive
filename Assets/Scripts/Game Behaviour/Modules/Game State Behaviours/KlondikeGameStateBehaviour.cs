namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	

	[CreateAssetMenu(fileName = "KlondikeGameStateBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Game State Behaviours/Klondike")]
	public class KlondikeGameStateBehaviour : BaseGameStateBehaviour
	{
		[SerializeField] int cardDrawCount = 3;
		public override bool IsGameStuck()
		{
			ZoneParent waste = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0];
			Deck deck = GameBoard.Instance.GetDeck();

			// Confirm that we are in a state where we can accurately check if we are stuck
			// We can only check if there are no cards in the waste due to ease of calculations
			if (waste.CardCount > 0)
				return false;

				List<Card> cardsToCheck = new();
			foreach (Card card in GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau).Select(o => o.BottomCard))
			{
				if (card != null)
					cardsToCheck.AddRange(BaseGameRules.ActiveRules.GetCardChain(card));
			}

			// Can we move any of the currently visible cards?
			foreach (Card card in cardsToCheck)
			{
				if (card != null)
				{
					List<ZoneParent> possibleMoves = StandardGameManager.Instance.GetPossibleMoves(card);
					if (possibleMoves.Count > 0)
					{
						// Ensure that this isn't just moving in a way that makes no difference (for example, moving a card from one foundation to another)
						ZoneParent cardParent = card.GetZoneParent();
						foreach (ZoneParent parent in possibleMoves)
						{
							if (cardParent.Zone != parent.Zone)
							{
								return false;
							}
							else
							{
								// If we are moving with the Tableau, make sure we are moving to a card that is different to the card we are currently under
								if (parent.Zone == GameBoard.CardZone.Tableau)
								{
									if (card.linkedObj.TryGetParentCard(out CardObject parentCard))
									{
										// Even if moving does nothing in the grand scheme of things, it is still a move that changes the game state
										if (parentCard.Flipped == false)
											return false;

										if (parent.CardCount > 0)
										{
											if (parent.BottomCard.Rank != parentCard.Rank || Card.SuitColors[parent.BottomCard.Suit] != Card.SuitColors[parentCard.Suit])
												return false;
										}
										else
										{
											return false;
										}
									}
									else
									{
										// In this case, return false unless we are a king (since the only valid Tableau move for a king is going to another empty space
										if (card.Rank != Card.CardRank.King)
										{
											return false;
										}
									}
								}
								else // moving within the Foundation is useless
								{
									return false;
								}
							}
						}
					}
				}
			}

			// Check if there are any possible moves if we draw cards
			// This function only runs when there are no cards in the waste, so no need to check waste
			cardsToCheck.Clear();
			if (cardDrawCount == 3)
			{
				if (deck.RemainingCards > 2)
				{
					for (int i = deck.RemainingCards - 3; i >= 0;)
					{
						cardsToCheck.Add(deck.Cards[i]);
						i -= 3;

						if (i < 0 && deck.RemainingCards % 3 != 0)
						{
							cardsToCheck.Add(deck.Cards[0]);
						}
					}
				}
				else if (deck.RemainingCards > 0)
				{
					cardsToCheck.Add(deck.Cards[0]);
				}
			}
			else
			{
				cardsToCheck.AddRange(deck.Cards);
			}


			foreach (Card card in cardsToCheck)
			{
				if (card != null)
				{
					if (StandardGameManager.Instance.GetPossibleMoves(card, true).Count > 0)
					{
						return false;
					}
				}
			}

			return true;
		}
	}

}