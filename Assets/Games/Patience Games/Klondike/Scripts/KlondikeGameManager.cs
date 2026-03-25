namespace CardGameArchive.Solitaire.Klondike
{
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Linq;
	using static Deck;

	public class KlondikeGameManager : BaseGameManager
	{
		public override async void StartGame()
		{
			GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Stock)[0].GetComponent<DeckObject>().InitializeDeck(Deck);

			Deck.Initialise(DeckType.Full52);
			Deck.Shuffle();

			for (int i = 0; i < 7; i++)
			{
				for (int j = i; j < 7; j++)
				{
					Card card = Deck.Draw();
					card.interactable = false;
					GameBoard.Instance.PlaceCard
						(
							card: card,
							destination: GameBoard.CardZone.Tableau,
							index: j,
							fromStock: true
						);

					// Last card in column
					if (j == i)
					{
						card.interactable = true;
						card.SetFlipped(true);
					}

					await Task.Delay(100);
				}
			}
		}

		public override void OnCardTapped(Card card)
		{
			// Search to see where it could be placed and then move it there if possible			
			// 1. Calculate all possible placements
			//		a. Can only be in Tableau or Foundation
			//		b. If in Foundation, can only be placed in Tableau
			// 2. Figure out the best one, or at least, the most logical one

			
		}
		public override void OnCardGrabbed(Card card)
		{
			// Move card above everything else
			card.linkedObj.transform.position = new(card.linkedObj.transform.position.x, card.linkedObj.transform.position.y, -1);
		}
		public override void OnCardDropped(Card card)
		{
			// Decide if the card can be placed here, if not, move it back
		}

		public override void OnDeckClicked(Deck deck)
		{
			if (deck.RemainingCards > 0)
			{
				for (int i = 0; i < 3; i++)
				{
					Card card = Deck.Draw();

					// This means we've reached the end of the deck
					if (card == null)
						break;

					GameBoard.Instance.PlaceCard(card, GameBoard.CardZone.Waste, 
													fromStock: true);
					
					card.SetFlipped(true);
					card.interactable = false;
				}

				Transform wasteZone = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0].transform;
				wasteZone.GetChild(wasteZone.childCount-1).GetComponent<CardObject>().cardData.interactable = true;
			}

			// Return all cards in waste into deck
			else
			{
				Transform waste = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0].transform;
				List<CardObject> cards = waste.GetAllChildren().Select(o => o.GetComponent<CardObject>()).Where(o => o != null).ToList();

				cards.Reverse();
				foreach (CardObject card in cards)
				{
					GameBoard.Instance.PlaceCard(card.cardData, GameBoard.CardZone.Stock);
					card.cardData.SetFlipped(false);
				}
			}
		}
	}
}
