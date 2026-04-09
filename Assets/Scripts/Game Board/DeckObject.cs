namespace CardGameArchive
{
    using UnityEngine;

	public class DeckObject : MonoBehaviour, ITappable
	{
		[SerializeField] Deck.DeckType deckType;
		public Deck Data { get; private set; } = new();

		SpriteRenderer sRenderer;

        void Awake()
        {
            sRenderer = GetComponent<SpriteRenderer>();
			InitializeDeck();
		}
        void InitializeDeck()
		{
			Data.Initialise(deckType, this);
		}

		public void OnTap()
		{
			BaseGameManager.Instance.OnDeckTapped(Data);
		}

		public void SetVisible()
		{
			sRenderer.sprite = Data.RemainingCards > 0 ? CardSpriteCollection.Instance.GetCardBack() : CardSpriteCollection.Instance.GetEmptyCard();
		}
	}

}