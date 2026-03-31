namespace CardGameArchive
{
	using System.Threading.Tasks;
	using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class CardObject : MonoBehaviour, ITappable, IDraggable
    {
        public SpriteRenderer spriteRenderer { get; private set; }
        public new Collider2D collider { get; private set; }

        public Card Data { get; private set; }
        public Card.CardSuit Suit => Data.Data.suit;
        public Card.CardRank Rank => Data.Data.rank;

        Vector3 destination = Vector3.zero;
        public bool Moving { get; private set; } = false;

		void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
        }

        void Update()
        {
            if (transform.localPosition != destination)
            {
                if (!Moving && Vector3.Distance(transform.localPosition, destination) < 0.01f)
                {
                    transform.localPosition = destination;
                }
            }
        }

        public void InitialiseCard(Card card)
        {
            Data = card;
            Data.SetFlipped(false);
        }

        public async Task MoveCard(Vector3 destination, float timeToMove = 0, bool teleport = false)
        {
            this.destination = destination;

            if (teleport)
            {
                transform.localPosition = destination;
            }

            if (!Moving)
            {
                await Move(timeToMove);
            }
		}

        async Task Move(float timeToMove)
        {
            Moving = true;
			if (timeToMove <= 0)
			{
				transform.localPosition = destination;
                Moving = false;
				return;
			}

            Vector3 currentDestination = destination;

			float moveSpeed = Vector3.Distance(transform.localPosition, destination) / timeToMove;
			moveSpeed = Mathf.Max(moveSpeed, 0.1f);

			while (Vector3.Distance(transform.localPosition, destination) > moveSpeed * 0.01f)
			{
                if (currentDestination != destination)
                {
                    currentDestination = destination;
                    moveSpeed = Vector3.Distance(transform.localPosition, destination) / timeToMove;
                    moveSpeed = Mathf.Max(moveSpeed, 0.1f);
				}

				transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, moveSpeed * Time.deltaTime);
				await Task.Yield();
			}
			transform.localPosition = destination;
            Moving = false;
		}

        public ZoneParent GetZoneParent()
        {
            return transform.GetComponentInParent<ZoneParent>();
        }

		public void OnTap()
		{
            BaseGameManager.Instance.OnCardTapped(Data);
		}

		public void OnGrab()
		{
            BaseGameManager.Instance.OnCardGrabbed(Data);
		}

		public void OnDrop()
		{
            BaseGameManager.Instance.OnCardDropped(Data);
		}
	}
}
