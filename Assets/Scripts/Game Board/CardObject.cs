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
        public Card.CardSuit Suit => Data.Suit;
        public Card.CardRank Rank => Data.Rank;
        public int ID => Data.ID;
        public bool Flipped => Data.Flipped;

		Vector3 destination = Vector3.zero;
        public bool Moving { get; private set; } = false;
        public bool CanMove { get; private set; } = true;

        [SerializeField] float correctionMoveTime = 0.1f;

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
                        GameTaskManager.Instance.AddTask(Move(correctionMoveTime));
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
                return;
            }

            if (!Moving)
            {
                timeToMove = timeToMove < 0 ? correctionMoveTime : timeToMove;
                GameTaskManager.Instance.AddTask(Move(timeToMove));
                await GameTaskManager.Instance.WhenAll();
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
		public bool TryGetParentCard(out CardObject parentCard)
		{
            parentCard = transform.parent?.GetComponent<CardObject>();
            return parentCard != null;
		}
		public bool TryGetChildCard(out CardObject childCard)
		{
            foreach (Transform child in transform)
            {
                childCard = child.GetComponent<CardObject>();
                if (childCard != null)
                    return true;
            }

			childCard = null;
			return false;
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
            public Card.CardData cardData = new();
            public GameBoard.CardZone zone;
            public int zoneIndex;
            public bool flipped;
            public bool interactable;
            public bool canMove;
        }
		public SaveData Save()
		{
            CardSaveData data = new();
            data.zone = GetZoneParent().Zone;
            data.zoneIndex = GameBoard.Instance.GetZoneIndex(GetZoneParent());
			data.cardData = new Card.CardData(Rank, Suit, Data.ID);
            data.flipped = Data.Flipped;
            data.interactable = Data.Interactable;
            data.canMove = CanMove;
            return data;
		}

		public void Load(SaveData saveData)
		{
            CardSaveData cardData = saveData as CardSaveData;
            Data.SetData(cardData.cardData.rank, cardData.cardData.suit);
            GameTaskManager.Instance.AddTask(Data.SetFlipped(cardData.flipped, instant: true));
            Data.SetInteractable(cardData.interactable);
            Data.SetCardSprite();
            CanMove = cardData.canMove;
		}
	}
}
