namespace CardGameArchive
{
	using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class CardObject : MonoBehaviour, ITappable, IDraggable
    {
        public SpriteRenderer spriteRenderer { get; private set; }

        public Card cardData { get; private set; }

        public Card.CardData Data => cardData.Data;
        public Card.CardSuit Suit => cardData.Data.suit;
        public Card.CardRank Rank => cardData.Data.rank;

		void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void InitialiseCard(Card card)
        {
            cardData = card;
            cardData.SetFlipped(false);
        }

		public void OnTap()
		{
            BaseGameManager.Instance.OnCardTapped(cardData);
		}

		public void OnGrab()
		{
            BaseGameManager.Instance.OnCardGrabbed(cardData);
		}

		public void OnDrop()
		{
            BaseGameManager.Instance.OnCardDropped(cardData);
		}
	}
}
