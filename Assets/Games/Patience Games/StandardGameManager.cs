namespace CardGameArchive
{
	using CardGameArchive.Behaviours;
	using CardGameArchive.Rules;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using UnityEngine;

	/// <summary>
	/// Base class for handling all code related to managing the card game
	/// Manages the vast majority of the game logic and serves as a mediator between different systems
	/// </summary>
	public class StandardGameManager : MonoBehaviour, ISaveable
	{
		public static StandardGameManager Instance { get; private set; }

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

		[field: SerializeField] protected BaseDeckVerificationBehaviour DeckVerifierBehaviour { get; private set; }
		[field: SerializeField] protected BaseGameDealBehaviour DealSetupBehaviour { get; private set; }			
		[field: SerializeField] protected List<BasePostSetupBehaviour> PostSetupBehaviour { get; private set; }			

		[field: SerializeField] protected BaseMoveBehaviour MoveBehaviour { get; private set; }

		[field: SerializeField] protected BaseGameStateBehaviour GameStateBehaviour { get; private set; }

		[field: SerializeField] protected BaseGameInputBehaviour GameInputBehaviour { get; private set; }

		[field: SerializeField] protected BaseDeckBehaviour DeckBehaviour { get; private set; }

		[field: SerializeField] protected List<BaseCardEventBehaviour> CardEventBehaviour { get; private set; }

		[field: SerializeField] protected BaseUndoBehaviour UndoBehaviour { get; private set; }

		[field: SerializeField] protected BaseScoreBehaviour ScoreBehaviour { get; private set; }

		[field: SerializeField] protected List<BasePostLoadBehaviour> PostLoadBehaviour { get; private set; }

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		protected virtual async void Start()
		{
			SetRules();

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

				DeckVerifierBehaviour.Verify();
				GameBoard.Instance.GenerateCards();

				LoadingScreen.Instance.Hide();

				GameTaskManager.Instance.QueueTask(() => DealSetupBehaviour.DealCards());

				await GameTaskManager.Instance.WhenAll();

				foreach (var behaviour in PostSetupBehaviour)
				{
					behaviour.FinaliseBoard();
				}
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

		void SetRules()
		{
			Rules = Name switch
			{
				GameTerms.GameName.KlondikeDealOne => new KlondikeGameRules(),
				GameTerms.GameName.KlondikeDealThree => new KlondikeGameRules(),

				GameTerms.GameName.SpiderOneSuit => new SpiderGameRules(),
				GameTerms.GameName.SpiderTwoSuit => new SpiderGameRules(),
				GameTerms.GameName.SpiderFourSuit => new SpiderGameRules(),

				GameTerms.GameName.SpideretteOneSuit => new SpideretteGameRules(),
				GameTerms.GameName.SpideretteTwoSuit => new SpideretteGameRules(),
				GameTerms.GameName.SpideretteFourSuit => new SpideretteGameRules(),

				GameTerms.GameName.Clock => new ClockGameRules(),
				_ => throw new NotImplementedException()
			};
		}
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
			GameTaskManager.Instance.OnTasksFinished += MoveBehaviour.AutoMove;
			GameTaskManager.Instance.OnTasksFinished += CheckGameState;
		}
		protected virtual void UnlinkEvents()
		{
			OnInvalidAction -= AudioManager.Instance.OnInvalidAction;
			OnInvalidAction -= FeedbackManager.Instance.OnInvalidAction;

			GameBoard.Instance.OnCardMoveStart -= AudioManager.Instance.OnCardMove;
			GameBoard.Instance.OnCardMoveStart -= OnCardMoveStart;

			GameBoard.Instance.OnCardMoveFinish -= OnCardMoveFinish;

			GameTaskManager.Instance.OnTaskAdded -= InputManager.Instance.DisableInput;
			GameTaskManager.Instance.OnTaskAdded -= UIManager.Instance.DisableUI;

			GameTaskManager.Instance.OnTasksFinished -= InputManager.Instance.EnableInput;
			GameTaskManager.Instance.OnTasksFinished -= UIManager.Instance.EnableUI;
			GameTaskManager.Instance.OnTasksFinished -= MoveBehaviour.AutoMove;
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
			UIManager.Instance.DisableUI();
			SaveManager.ClearGameSave(Name);
			CanSave = false;

			await Awaitable.WaitForSecondsAsync(2f);

			await UIManager.Instance.ShowWinScreenAsync();
			UIManager.Instance.EnableUI();
		}
		protected virtual async void OnGameLose()
		{
			GamePlaying = false;
			InputManager.Instance.DisableInput();
			UIManager.Instance.DisableUI();
			SaveManager.ClearGameSave(Name);
			CanSave = false;

			await Awaitable.WaitForSecondsAsync(2f);
			

			await UIManager.Instance.ShowLoseScreenAsync();
			UIManager.Instance.EnableUI();
		}
		public void MoveTaken(GameMove move)
		{
			if (!GamePlaying || UndoBehaviour == null)
				return;

			gameMoves.Push(move);
		}
		public void InvokeInvalidAction() => OnInvalidAction?.Invoke(null);
		public void InvokeInvalidAction(Card card) => OnInvalidAction?.Invoke(card);
		public void InvokeUndo(GameMove move) { OnUndo?.Invoke(move); }

		// Passthrough functions
		public void OnDeckTapped(Deck deck) => DeckBehaviour.OnDeckTapped(deck);
		public void OnCardTapped(Card card) => GameInputBehaviour.OnCardTapped(card);
		public void OnCardGrabbed(Card card) => GameInputBehaviour.OnCardGrabbed(card);
		public void OnCardDropped(Card card) => GameInputBehaviour.OnCardDropped(card);
		public async Task MoveCardToBestDestination(Card card) => await MoveBehaviour.MoveCardToBestDestination(card);
		public List<ZoneParent> GetPossibleMoves(Card card, bool simulation = false) => MoveBehaviour.GetPossibleMoves(card, simulation);
		public async Task UndoMove()
		{
			Task undoTask = UndoBehaviour.UndoMove(gameMoves);
			GameTaskManager.Instance.AddTask(undoTask);
			await undoTask;
		}
		public void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.canUndo)
				MoveTaken(new(GameMove.MoveType.CardMoved, new GameMove.CardMovedData(eventData.card, eventData.from, eventData.to, eventData.contingent)));

			foreach (var behaviour in CardEventBehaviour)
			{
				behaviour.OnCardMoveStart(eventData);
			}
		}
		public void OnCardMoveFinish(GameBoard.CardMoveEvent data)
		{
			foreach (var behaviour in CardEventBehaviour)
			{
				behaviour.OnCardMoveFinish(data);
			}
		}

		public int GetScore() => ScoreBehaviour.GetScore();

		protected virtual void OnDisable()
		{
			UnlinkEvents();
		}

		public class GameManagerSaveData : SaveData
		{
			public float gameTime;
			public List<SaveData> gameMoves = new();
			public GameManagerSaveData(float gameTime) { this.gameTime = gameTime; }
		}
		public virtual SaveData Save()
		{
			GameManagerSaveData data = new(GameTime);
			foreach (GameMove move in gameMoves)
			{
				data.gameMoves.Add(move.Save());
			}
			return data;
		}
		public virtual void Load(SaveData saveData)
		{
			GameManagerSaveData gameSaveData = (saveData as GameSaveData).gameManagerData as GameManagerSaveData;
			GameTime = gameSaveData.gameTime;
			gameBoard.Load((saveData as GameSaveData).gameBoardData);

			List<GameMove.GameMoveSaveData> moveSaveData = gameSaveData.gameMoves.OfType<GameMove.GameMoveSaveData>().ToList();
			moveSaveData.Reverse();
			gameMoves.Clear();
			foreach (var moveData in moveSaveData)
			{
				GameMove gameMove = new GameMove();
				gameMove.Load(moveData);
				gameMoves.Push(gameMove);
			}

			if (!loadFailed)
			{
				try
				{
					foreach (var behaviour in PostLoadBehaviour)
					{
						if (!behaviour.PostLoad(saveData))
						{
							throw new System.Exception("Post load behaviour failed");
						} 
					}
				}
				catch (Exception e)
				{
					LoadFailed(e.Message);
				}
			}
		}

		public virtual async void LoadFailed(string reason)
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