namespace CardGameArchive
{
	using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class CardObject : MonoBehaviour, ITappable, IDraggable
    {
        public SpriteRenderer spriteRenderer { get; private set; }
        public new Collider2D collider { get; private set; }

        public Card CardData { get; private set; }

        public Card.CardData Data => CardData.Data;
        public Card.CardSuit Suit => CardData.Data.suit;
        public Card.CardRank Rank => CardData.Data.rank;

		void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
        }

        public void InitialiseCard(Card card)
        {
            CardData = card;
            CardData.SetFlipped(false);
        }

		public void OnTap()
		{
            BaseGameManager.Instance.OnCardTapped(CardData);
		}

		public void OnGrab()
		{
            BaseGameManager.Instance.OnCardGrabbed(CardData);
		}

		public void OnDrop()
		{
            BaseGameManager.Instance.OnCardDropped(CardData);
		}
	}
}
