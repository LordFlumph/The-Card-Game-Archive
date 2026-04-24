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

		public float GameTime { get; private set; } = 0f;

		[field: SerializeField] public bool UseScore { get; protected set; }

		[field: SerializeField] public bool CanSave { get; protected set; } = true;
		public bool GameStarted { get; protected set; } = false;

		protected GameBoard gameBoard { get { return GameBoard.Instance; } }

		protected Stack<GameMove> gameMoves = new();
		public bool CanUndo => gameMoves.Count > 0;

		public event Action<Card> OnInvalidAction;
		public event Action<GameMove> OnUndo;

		bool loadFailed = false;

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

			if (CanSave)
			{
				GameSaveData saveData = SaveManager.LoadGame(Name);
				if (saveData != null)
				{
					Load(saveData);

					if (loadFailed)
						return;

					LinkEvents();

					GameTaskManager.Instance.AddTask(UIManager.Instance.ShowLoadConfirmationAsync());
					await GameTaskManager.Instance.WhenAll();
					LoadingScreen.Instance.Hide();

					Debug.Log("Load finished");
				}
				else
				{
					LinkEvents();
					LoadingScreen.Instance.Hide();
					await GameTaskManager.Instance.WhenAll();
					StartGame();
				}
			}
			else
			{
				LinkEvents();
				LoadingScreen.Instance.Hide();
				await GameTaskManager.Instance.WhenAll();
				StartGame();
			}

			await GameTaskManager.Instance.WhenAll();
			GameStarted = true;
		}

		protected virtual void Update()
		{
			GameTime += Time.deltaTime;
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
			GameTaskManager.Instance.OnTasksFinished += AutoMoveAny;
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
			GameTaskManager.Instance.OnTasksFinished -= AutoMoveAny;
		}
		protected abstract Task StartGame();
		public virtual void RestartGame()
		{
			SaveManager.ClearGameSave(Name);
			GameSceneManager.Instance.ReloadScene();
		}
		protected virtual bool VerifyDeck() => true;
		public abstract void OnDeckTapped(Deck deck);
		public abstract void OnCardTapped(Card card);
		public virtual void OnCardGrabbed(Card card) { }
		public abstract void OnCardDropped(Card card);
		public abstract List<ZoneParent> GetPossibleMoves(Card card, bool skipCardCanMove = false);

		/// <summary>
		/// Automatically move any cards that can be moved
		/// </summary>
		public virtual void AutoMoveAny() { throw new System.NotImplementedException(); }

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

		public virtual int GetScore() => 0;

		protected virtual void OnDisable()
		{
			UnlinkEvents();
		}

		public abstract class BaseGameSaveData : SaveData
		{
			public float gameTime;
			public BaseGameSaveData(float gameTime) { this.gameTime = gameTime; }
		}
		public abstract SaveData Save();
		public virtual void Load(SaveData saveData)
		{
			GameTime = ((saveData as GameSaveData).gameManagerData as BaseGameSaveData).gameTime;
			gameBoard.Load((saveData as GameSaveData).gameBoardData);
		}

		public async void LoadFailed(string reason)
		{
			Debug.LogError($"Unable to load save data: {reason}");
			loadFailed = true;

			await GameTaskManager.Instance.WhenAll();
			await Task.Delay(200);

			SaveManager.ClearGameSave(Name);
			GameSceneManager.Instance.ReloadScene();
		}
	}
}
