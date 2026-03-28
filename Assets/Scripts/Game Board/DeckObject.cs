namespace CardGameArchive
{
    using UnityEngine;

    public class DeckObject : MonoBehaviour, ITappable
    {
		private Deck deckData = null;

		SpriteRenderer sRenderer;

        void Awake()
        {
            sRenderer = GetComponent<SpriteRenderer>();
        }
        public void InitializeDeck(Deck deck)
		{
			deckData = deck;
		}

		public void OnTap()
		{
			BaseGameManager.Instance.OnDeckTapped(deckData);
		}

		public void SetVisible()
		{
			sRenderer.sprite = deckData.RemainingCards > 0 ? CardSpriteCollection.Instance.GetCardBack() : CardSpriteCollection.Instance.GetEmptyCard();
		}
    }

}