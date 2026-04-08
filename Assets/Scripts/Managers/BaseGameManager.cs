namespace CardGameArchive
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	/// <summary>
	/// Base class for handling all code related to managing the card game
	/// Manages the vast majority of the game logic and serves as a mediator between different systems
	/// </summary>
	public abstract class BaseGameManager : MonoBehaviour, ISaveable
	{
		public static BaseGameManager Instance { get; private set; }

		public BaseGameRules Rules { get; protected set; }
		public GameTerms.GameName Name { get; protected set; }

		public bool CanSave { get; protected set; } = true;
		public bool GameStarted { get; protected set; } = false;

		protected GameBoard gameBoard { get { return GameBoard.Instance; } }

		protected Stack<GameMove> gameMoves = new();
		public bool CanUndo => gameMoves.Count > 0;

		public event Action<Card> OnInvalidAction;
		public event Action<GameMove> OnUndo;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		protected virtual async void Start()
		{
			SetGame();
			LinkEvents();

			if (CanSave)
			{
				GameSaveData saveData = null;//SaveManager.LoadGame(Name);
				try
				{
					if (saveData != null)
					{
						Load(saveData);
					}
					else
					{
						await StartGame();
					}
				}
				catch (Exception e)
				{
					Debug.LogError("Error loading save data, starting new game. Exception: " + e);
					await StartGame();
				}
			}
			else
			{
				await StartGame();
			}
			

			UIManager.Instance?.EnableUI();

			GameStarted = true;
		}

		protected abstract void SetGame();
		protected virtual void LinkEvents()
		{			
			OnInvalidAction += AudioManager.Instance.OnInvalidAction;
			OnInvalidAction += FeedbackManager.Instance.OnInvalidAction;

			GameBoard.Instance.OnCardMoveStart += AudioManager.Instance.OnCardMove;
			GameBoard.Instance.OnCardMoveStart += OnCardMoveStart;

			GameBoard.Instance.OnCardMoveFinish += OnCardMoveFinish;

			GameTaskManager.Instance.OnTaskAdded += InputManager.Instance.DisableInput;
			GameTaskManager.Instance.OnTaskAdded += UIManager.Instance.DisableUI;

			GameTaskManager.Instance.OnTasksFinished += InputManager.Instance.EnableInput;
			GameTaskManager.Instance.OnTasksFinished += UIManager.Instance.EnableUI;
		}
		protected virtual void UnlinkEvents()
		{			
			OnInvalidAction -= AudioManager.Instance.OnInvalidAction;
			OnInvalidAction += FeedbackManager.Instance.OnInvalidAction;

			GameBoard.Instance.OnCardMoveStart -= AudioManager.Instance.OnCardMove;
			GameBoard.Instance.OnCardMoveStart -= OnCardMoveStart;

			GameBoard.Instance.OnCardMoveFinish -= OnCardMoveFinish;

			GameTaskManager.Instance.OnTaskAdded -= InputManager.Instance.DisableInput;
			GameTaskManager.Instance.OnTaskAdded -= UIManager.Instance.DisableUI;

			GameTaskManager.Instance.OnTasksFinished -= InputManager.Instance.EnableInput;
			GameTaskManager.Instance.OnTasksFinished -= UIManager.Instance.EnableUI;
		}
		protected abstract Task StartGame();
		public abstract void RestartGame();
		protected virtual bool VerifyDeck() => true;
		public abstract void OnDeckTapped(Deck deck);
		public abstract void OnCardTapped(Card card);
		public virtual void OnCardGrabbed(Card card) { }
		public abstract void OnCardDropped(Card card);
		public abstract List<ZoneParent> GetPossibleMoves(Card card);

		/// <summary>
		/// Automatically move any cards that can be moved
		/// </summary>
		public virtual async Task AutoMoveCards() { throw new System.NotImplementedException(); }

		/// <summary>
		/// Automatically move this card somewhere it will 
		/// </summary>
		/// <param name="card"></param>
		/// <param name="playerDriven">Whether this move was triggered directly by the player</param>
		/// <returns></returns>
		public virtual async Task AutoMove(Card card, bool playerDriven = true) { }
		public virtual bool IsGameStuck() => false;

		protected virtual void OnCardMoveStart(GameBoard.CardMoveEvent eventData) { }
		protected virtual void OnCardMoveFinish(GameBoard.CardMoveEvent eventData) { }

		public virtual async Task UndoMove() { await Task.CompletedTask; }

		protected void InvokeInvalidAction(Card card) { OnInvalidAction?.Invoke(card); }
		protected void InvokeUndo(GameMove move) { OnUndo?.Invoke(move); }

		protected virtual void OnDisable()
		{
			UnlinkEvents();
		}

		public abstract SaveData Save();
		public abstract void Load(SaveData saveData);
	}
}
