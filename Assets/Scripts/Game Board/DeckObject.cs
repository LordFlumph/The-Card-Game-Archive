namespace CardGameArchive
{
    using UnityEngine;

    public class DeckObject : MonoBehaviour, ITappable
    {
		private Deck deckData = null;
		public void InitializeDeck(Deck deck)
		{
			deckData = deck;
		}

		public void OnTap()
		{
			BaseGameManager.Instance.OnDeckClicked(deckData);
		}
    }

}