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

		protected Stack<GameMove> gameMoves = new();

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
			LinkEvents();
			StartGame();
		}

		protected abstract void SetRules();
		protected virtual void LinkEvents()
		{
			GameBoard.Instance.OnCardMoveStart += AudioManager.Instance.OnCardMove;
		}
		protected abstract void StartGame();
		public abstract void OnDeckTapped(Deck deck);
		public abstract void OnCardTapped(Card card);
		public abstract void OnCardGrabbed(Card card);
		public abstract void OnCardDropped(Card card);
		public abstract List<ZoneParent> GetPossibleMoves(Card card);
		public virtual async Task AutoMoveCards() { }
		public virtual async Task AutoMove(Card card) { }
		public virtual bool IsGameStuck() => false;

		protected virtual void OnCardMoveStart(GameBoard.CardMoveEvent eventData) { }
		protected virtual void OnCardMoveFinish(GameBoard.CardMoveEvent eventData) { }

		public virtual void UndoMove() { }

		public virtual void RestartGame()
		{
			GameSceneManager.Instance.ReloadScene();
		}
	}
}
