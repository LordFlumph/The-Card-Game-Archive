namespace CardGameArchive.Solitaire.Klondike
{
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using UnityEngine;
	using static Deck;

	public class KlondikeGameManager : BaseGameManager
	{
		public override async void StartGame()
		{
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

			List<Transform> possibleZones = new();
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
			// Usually, draw a card
		}


	}
}
