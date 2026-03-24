namespace CardGameArchive
{
	using Unity.VisualScripting;
	using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class CardObject : MonoBehaviour, IClickable, IDraggable
    {
        private SpriteRenderer spriteRenderer;

        public Card cardData { get; private set; }

        public Card.CardData Data => cardData.Data;

		void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void InitialiseCard(Card card)
        {
            cardData = card;
            SetFlipped(false);
        }

        public void SetFlipped(bool flipped)
        {
            if (!flipped)
            {
                spriteRenderer.sprite = CardSpriteCollection.Instance.GetCardBack();
            }
            else
            {
                if (cardData == null)
                {
                    Debug.LogError("Attempting to flip card that has invalid card data");
                    return;
                }

                spriteRenderer.sprite = CardSpriteCollection.Instance[cardData.Data];
            }
        }

		public void OnClick()
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
