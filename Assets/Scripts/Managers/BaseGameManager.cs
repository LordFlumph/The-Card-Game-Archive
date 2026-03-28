namespace CardGameArchive
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class BaseGameManager : MonoBehaviour
	{
		public static BaseGameManager Instance { get; private set; }

		public BaseGameRules Rules { get; protected set; }

		protected Deck Deck { get; } = new Deck();

		protected GameBoard gameBoard { get { return GameBoard.Instance; } }

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		protected virtual void Start()
		{
			SetRules();
			StartGame();
		}

		public abstract void SetRules();
		public abstract void StartGame();
		public abstract void OnDeckTapped(Deck deck);
		public abstract void OnCardTapped(Card card);
		public abstract void OnCardGrabbed(Card card);
		public abstract void OnCardDropped(Card card);
		public abstract List<ZoneParent> GetPossibleMoves(Card card);
		public virtual async Task AutoMoveCards() { }
		public virtual async Task AutoMove(Card card) { }

		protected virtual void OnCardMoveStart(GameBoard.CardMoveEvent eventData) { }
		protected virtual void OnCardMoveFinish(GameBoard.CardMoveEvent eventData) { }
	}
}
