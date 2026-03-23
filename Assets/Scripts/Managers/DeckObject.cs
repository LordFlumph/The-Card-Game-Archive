namespace CardGameArchive
{
    using UnityEngine;

    public class DeckObject : MonoBehaviour, IClickable
    {
		private Deck deckData = null;
		public void InitializeDeck(Deck deck)
		{
			deckData = deck;
		}

		public void OnClick()
		{
			BaseGameManager.Instance.OnDeckClicked(deckData);
		}
    }

}