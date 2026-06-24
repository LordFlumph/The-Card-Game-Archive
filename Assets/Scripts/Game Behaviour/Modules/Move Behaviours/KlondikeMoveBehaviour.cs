namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	

	[CreateAssetMenu(fileName = "KlondikeMoveBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Move Behaviours/Klondike")]
	public class KlondikeMoveBehaviour : BaseMoveBehaviour
	{
		public override void AutoMove()
		{
			if (!CanAutoMove)
				return;

			List<Card> cardsToCheck = new();

			// In Deal Three, Automoving the waste card could block future moves. So we'll only automove it in the Deal One variant
			if (StandardGameManager.Instance.Variant == GameTerms.GameVariant.KlondikeDealOne)
			{
				Card wasteCard = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0].BottomCard;
				if (wasteCard != null)
					cardsToCheck.Add(wasteCard);
			}
			

			foreach (ZoneParent tableauParent in GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau))
			{
				if (tableauParent.BottomCard != null)
					cardsToCheck.Add(tableauParent.BottomCard);
			}

			// Find all valid Foundation moves
			List<(Card card, ZoneParent destination)> possibleMoves = new();

			foreach (Card card in cardsToCheck)
			{
				List<ZoneParent> validMoves = GetPossibleMoves(card, GameBoard.CardZone.Foundation);

				if (validMoves.Count > 0)
				{
					possibleMoves.Add((card, validMoves[0]));
				}
			}

			if (possibleMoves.Count == 0)
				return;

			// Track current foundation ranks
			Card.CardRank clubRank = Card.CardRank.Ace;
			Card.CardRank diamondRank = Card.CardRank.Ace;
			Card.CardRank heartRank = Card.CardRank.Ace;
			Card.CardRank spadeRank = Card.CardRank.Ace;

			foreach (ZoneParent foundation in GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation))
			{
				if (foundation.BottomCard != null)
				{
					switch (foundation.BottomCard.Suit)
					{
						case Card.CardSuit.Clubs:
							clubRank = foundation.BottomCard.Rank;
							break;
						case Card.CardSuit.Diamonds:
							diamondRank = foundation.BottomCard.Rank;
							break;
						case Card.CardSuit.Hearts:
							heartRank = foundation.BottomCard.Rank;
							break;
						case Card.CardSuit.Spades:
							spadeRank = foundation.BottomCard.Rank;
							break;
					}
				}
			}

			// Remove moves that could block future plays
			for (int i = possibleMoves.Count - 1; i >= 0; i--)
			{
				Card card = possibleMoves[i].card;

				// Aces and Twos are always safe to move
				if (card.Rank is Card.CardRank.Ace or Card.CardRank.Two)
					continue;

				// Remove moves which could result in a card being blocked later
				// Rules are that a card can be safely moved if all cards of the opposite colour and one rank lower are already in the foundation
				if (Card.SuitColors[card.Suit] == Card.CardColor.Red)
				{
					if (BaseGameRules.ActiveRules.GetRankValue(card.Rank) - BaseGameRules.ActiveRules.GetRankValue(clubRank) > 1 ||
						BaseGameRules.ActiveRules.GetRankValue(card.Rank) - BaseGameRules.ActiveRules.GetRankValue(spadeRank) > 1)
					{
						possibleMoves.RemoveAt(i);
					}
				}
				else
				{
					if (BaseGameRules.ActiveRules.GetRankValue(card.Rank) - BaseGameRules.ActiveRules.GetRankValue(diamondRank) > 1 ||
						BaseGameRules.ActiveRules.GetRankValue(card.Rank) - BaseGameRules.ActiveRules.GetRankValue(heartRank) > 1)
					{
						possibleMoves.RemoveAt(i);
					}
				}
			}

			// Of the remaining moves, move the lowest ranked card to the foundation
			if (possibleMoves.Count > 0)
			{
				possibleMoves = possibleMoves.OrderBy(o => o.card.Rank).ToList();
				GameTaskManager.Instance.AddTask(RunAutoMove(possibleMoves[0].card, possibleMoves[0].destination));
			}
		}
	}

}