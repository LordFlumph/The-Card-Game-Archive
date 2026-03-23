namespace CardGameArchive.Solitaire.Klondike
{
	using UnityEngine;
	using static Deck;

	public class KlondikeGameManager : BaseGameManager
	{
		public override void StartGame()
		{
			Deck.Initialise(DeckType.Full52);
			Deck.Shuffle();

			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j <= i; j++)
				{
					Card card = Deck.Draw();
					card.interactable = false;
					GameBoard.Instance.PlaceCard
						(
							card: card,
							destination: GameBoard.CardDestination.Tableau,
							index: i,
							fromStock: true
						);

					// Last card in column
					if (j + 1 >= i)
					{
						card.interactable = true;
					}
				}
			}
		}

		public override void OnCardClicked(Card card)
		{
			// Search to see where it could be placed and then move it there if possible
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
