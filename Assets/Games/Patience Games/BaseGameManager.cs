#if true
namespace CardGameArchive.TMP
{
	using CardGameArchive.Behaviours;

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
		[field: SerializeField] public GameTerms.GameName Name { get; protected set; }

		public float GameTime { get; private set; } = 0f;

		[field: SerializeField] public bool UseScore { get; protected set; }

		[field: SerializeField] public bool CanSave { get; protected set; } = true;
		public bool GamePlaying { get; protected set; } = false;

		protected GameBoard gameBoard { get { return GameBoard.Instance; } }

		protected Stack<GameMove> gameMoves = new();
		public bool CanUndo => gameMoves.Count > 0;

		public event Action<Card> OnInvalidAction;
		public event Action<GameMove> OnUndo;

		protected bool loadFailed = false;

		[field: SerializeField] protected BaseGameSetupBehaviour SetupBehaviour { get; private set; }			

		[field: SerializeField] protected BaseMoveBehaviour MoveBehaviour { get; private set; }

		[field: SerializeField] protected BaseGameStateBehaviour GameStateBehaviour { get; private set; }

		[field: SerializeField] protected BaseGameInputBehaviour GameInputBehaviour { get; private set; }

		[field: SerializeField] protected BaseDeckBehaviour DeckBehaviour { get; private set; }

		[field: SerializeField] protected BaseCardEventBehaviour CardEventBehaviour { get; private set; }

		[field: SerializeField] protected BaseUndoBehaviour UndoBehaviour { get; private set; }

		[field: SerializeField] protected BaseScoreBehaviour ScoreBehaviour { get; private set; }

		[field: SerializeField] protected BasePostLoadBehaviour PostLoadBehaviour { get; private set; }

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

			bool loading = false;
			if (CanSave)
			{
				GameSaveData saveData = SaveManager.LoadGame(Name);
				if (saveData != null)
				{
					loading = true;
					Load(saveData);

					if (loadFailed)
						return;

					LinkEvents();

					GameTaskManager.Instance.AddTask(UIManager.Instance.ShowLoadConfirmationAsync());
					await GameTaskManager.Instance.WhenAll();
					LoadingScreen.Instance.Hide();

					Debug.Log("Load finished");
				}
			}

			if (!loading)
			{
				LinkEvents();
				LoadingScreen.Instance.Hide();
				GameTaskManager.Instance.QueueTask(() => SetupBehaviour.DealCards());
				await GameTaskManager.Instance.WhenAll();
				SetupBehaviour.FinaliseBoard();
			}

			await GameTaskManager.Instance.WhenAll();
			GamePlaying = true;
		}

		protected virtual void Update()
		{
			if (!GamePlaying)
				return;

			GameTime += Time.deltaTime;

			
		}

		protected abstract void SetGame();
		protected virtual void LinkEvents()
		{
			OnInvalidAction += AudioManager.Instance.OnInvalidAction;
			OnInvalidAction += FeedbackManager.Instance.OnInvalidAction;

			GameBoard.Instance.OnCardMoveStart += AudioManager.Instance.OnCardMove;
			GameBoard.Instance.OnCardMoveStart += CardEventBehaviour.OnCardMoveStart;

			GameBoard.Instance.OnCardMoveFinish += CardEventBehaviour.OnCardMoveFinish;

			GameTaskManager.Instance.OnTaskAdded += InputManager.Instance.DisableInput;
			GameTaskManager.Instance.OnTaskAdded += UIManager.Instance.DisableUI;

			GameTaskManager.Instance.OnTasksFinished += InputManager.Instance.EnableInput;
			GameTaskManager.Instance.OnTasksFinished += UIManager.Instance.EnableUI;
			GameTaskManager.Instance.OnTasksFinished += MoveBehaviour.AutoMoveAny;
			GameTaskManager.Instance.OnTasksFinished += CheckGameState;
		}
		protected virtual void UnlinkEvents()
		{
			OnInvalidAction -= AudioManager.Instance.OnInvalidAction;
			OnInvalidAction -= FeedbackManager.Instance.OnInvalidAction;

			GameBoard.Instance.OnCardMoveStart -= AudioManager.Instance.OnCardMove;
			GameBoard.Instance.OnCardMoveStart -= CardEventBehaviour.OnCardMoveStart;

			GameBoard.Instance.OnCardMoveFinish -= CardEventBehaviour.OnCardMoveFinish;

			GameTaskManager.Instance.OnTaskAdded -= InputManager.Instance.DisableInput;
			GameTaskManager.Instance.OnTaskAdded -= UIManager.Instance.DisableUI;

			GameTaskManager.Instance.OnTasksFinished -= InputManager.Instance.EnableInput;
			GameTaskManager.Instance.OnTasksFinished -= UIManager.Instance.EnableUI;
			GameTaskManager.Instance.OnTasksFinished -= MoveBehaviour.AutoMoveAny;
			GameTaskManager.Instance.OnTasksFinished -= CheckGameState;
		}
		public virtual async Task RestartGame()
		{
			List<ZoneParent> allZones = gameBoard.AllZoneParents;

			List<CardObject> cards = new();

			foreach (ZoneParent zone in allZones)
			{
				if (zone.Zone == GameBoard.CardZone.Stock)
					continue;

				cards.AddRange(zone.Cards);
				zone.RemoveAllCards();
			}

			foreach (CardObject card in cards)
			{
				GameTaskManager.Instance.AddTask(card.Data.SetFlipped(false));
				GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, GameBoard.CardZone.Stock, canUndo: false, affectCardChain: false));
			}

			gameMoves.Clear();

			await GameTaskManager.Instance.WhenAll();
			await Awaitable.WaitForSecondsAsync(0.2f);

			SaveManager.ClearGameSave(Name);
			GameSceneManager.Instance.ReloadScene();
		}
		protected virtual bool VerifyDeck() => true;
		protected virtual void CheckGameState()
		{
			if (Rules.IsWinConditionAchieved())
			{
				OnGameWin();
			}
			else if (Rules.IsLossConditionAchieved())
			{
				OnGameLose();
			}
			else if (GameStateBehaviour.IsGameStuck())
			{
				UIManager.Instance.ShowGameStuck();
			}
			else
			{
				UIManager.Instance.HideGameStuck();
			}
		}
		protected virtual async void OnGameWin()
		{
			GamePlaying = false;
			InputManager.Instance.DisableInput();
			SaveManager.ClearGameSave(Name);
			CanSave = false;

			await Awaitable.WaitForSecondsAsync(2f);

			UIManager.Instance.ShowWinScreenAsync();
		}
		protected virtual async void OnGameLose()
		{
			GamePlaying = false;
			InputManager.Instance.DisableInput();
			SaveManager.ClearGameSave(Name);
			CanSave = false;

			await Awaitable.WaitForSecondsAsync(2f);

			UIManager.Instance.ShowLoseScreenAsync();
		}
		public void InvokeInvalidAction() => OnInvalidAction?.Invoke(null);
		public void InvokeInvalidAction(Card card) => OnInvalidAction?.Invoke(card);
		public void InvokeUndo(GameMove move) { OnUndo?.Invoke(move); }

		// Passthrough functions
		public void OnDeckTapped(Deck deck) => DeckBehaviour.OnDeckTapped(deck);
		public void OnCardTapped(Card card) => GameInputBehaviour.OnCardTapped(card);
		public void OnCardDropped(Card card) => GameInputBehaviour.OnCardDropped(card);
		public async Task AutoMove(Card card) => await MoveBehaviour.AutoMove(card);
		public async Task UndoMove() => await UndoBehaviour.UndoMove(gameMoves);

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

			if (!loadFailed)
			{
				try
				{
					if (!PostLoadBehaviour.PostLoad(saveData))
					{
						throw new System.Exception("Post load behaviour failed");
					}
				}
				catch (Exception e)
				{
					LoadFailed(e.Message);
				}
			}
		}

		public async void LoadFailed(string reason)
		{
			Debug.LogError($"Unable to load save data: {reason}");
			loadFailed = true;

			await GameTaskManager.Instance.WhenAll();
			await Awaitable.WaitForSecondsAsync(0.2f);

			SaveManager.ClearGameSave(Name);
			GameSceneManager.Instance.ReloadScene();
		}
	}
}
#endif