namespace CardGameArchive
{
	using NUnit.Framework;
	using System;
    using System.Threading.Tasks;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    public class CardObject : MonoBehaviour, ITappable, IDraggable, ISaveable
    {
        public SpriteRenderer sRenderer { get; private set; }
        public new Collider2D collider { get; private set; }

        public Card Data { get; private set; }
        public Card.CardSuit Suit => Data.Data.suit;
        public Card.CardRank Rank => Data.Data.rank;

        Vector3 destination = Vector3.zero;
        public bool Moving { get; private set; } = false;
        public bool CanMove { get; private set; } = true;

        [SerializeField] float baseMoveSpeed = 0.1f;

        void Awake()
        {
            sRenderer = GetComponentInChildren<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
        }

        void Update()
        {
            if (CanMove && !Moving)
            {
                if (transform.localPosition != destination)
                {
                    if (Vector3.Distance(transform.localPosition, destination) < 0.01f)
                    {
                        transform.localPosition = destination;
                    }
                    else
                    {
                        Move(baseMoveSpeed);
                    }
                }
            }
        }

        public void InitialiseCard(Card card)
        {
            Data = card;
            Data.SetFlipped(false, true);
        }

        public async Task MoveCard(Vector3 destination, float timeToMove = -1, bool teleport = false)
        {
            this.destination = destination;

            if (teleport || timeToMove == 0)
            {
                transform.localPosition = destination;
            }

            if (!Moving)
            {
                timeToMove = timeToMove < 0 ? baseMoveSpeed : timeToMove;
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

                // Check if automove has been disabled, if so, just teleport to destination
                if (!CanMove)
                {
                    transform.localPosition = destination;
                    break;
                }

                transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, moveSpeed * Time.deltaTime);
                await Task.Yield();
            }
            transform.localPosition = destination;
            Moving = false;
        }


        public void SetAutoMove(bool autoMove)
        {
            CanMove = autoMove;
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

        public class CardSaveData : SaveData
        {
            
        }
		public SaveData Save()
		{
			throw new NotImplementedException();
		}

		public void Load()
		{
			throw new NotImplementedException();
		}
	}
}
