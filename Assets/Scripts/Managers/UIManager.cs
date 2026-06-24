namespace CardGameArchive
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Handles all code related to managing the UI
	/// </summary>
	public class UIManager : MonoBehaviour
	{
		public static UIManager Instance { get; private set; }

		[SerializeField] Button undoButton;

		[SerializeField] CanvasGroup winScreenGroup;
		[SerializeField] TextMeshProUGUI winScoreText, winScoreAmountText;
		[SerializeField] TextMeshProUGUI winTimeText;

		[SerializeField] CanvasGroup loseScreenGroup;
		[SerializeField] TextMeshProUGUI loseScoreText, loseScoreAmountText;
		[SerializeField] TextMeshProUGUI loseTimeText;

		[SerializeField] CanvasGroup confirmLoadGroup, confirmRestartGroup, confirmQuitGroup;

		[SerializeField] GameObject gameStuckObj;

		[SerializeField] GraphicRaycaster uiRaycaster;

		[SerializeField] float uiFadeTime = 0.2f;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		void Update()
		{
			if (StandardGameManager.Instance != null)
				undoButton.interactable = StandardGameManager.Instance.CanUndo;
		}

		public void EnableUI()
		{
			uiRaycaster.enabled = true;
		}
		public void DisableUI()
		{
			uiRaycaster.enabled = false;
		}

		public void Restart()
		{
			FadePopupsAsync();
			DisableUI();
			StandardGameManager.Instance.RestartGame();
		}
		public void Undo()
		{
			StandardGameManager.Instance.UndoMove();
		}
		public void Quit() => QuitAsync();

		async Task QuitAsync()
		{
			FadePopupsAsync();
			DisableUI();
			LoadingScreen.Instance.Show();
			await GameTaskManager.Instance.WhenAll();
			GameSceneManager.Instance.OpenMainMenu();
		}

		public void ShowLoadConfirmation() => ShowLoadConfirmationAsync();
		public async Task ShowLoadConfirmationAsync()
		{
			await confirmLoadGroup.FadeIn(uiFadeTime);
		}

		public void ShowRestartConfirmation() => ShowRestartConfirmationAsync();
		public async Task ShowRestartConfirmationAsync()
		{
			await confirmRestartGroup.FadeIn(uiFadeTime);
		}
		public void ShowQuitConfirmation() => ShowQuitConfirmationAsync();
		public async Task ShowQuitConfirmationAsync()
		{
			await confirmQuitGroup.FadeIn(uiFadeTime);
		}

		public void ShowWinScreen() => ShowWinScreenAsync();
		public async Task ShowWinScreenAsync()
		{
			if (StandardGameManager.Instance.UseScore)
			{
				int score = StandardGameManager.Instance.GetScore();
				winScoreAmountText.text = score.ToString();

				winScoreText.gameObject.SetActive(true);
				winScoreAmountText.gameObject.SetActive(true);
			}
			else
			{
				winScoreText.gameObject.SetActive(false);
				winScoreAmountText.gameObject.SetActive(false);
			}

			//winTimeText.text = StandardGameManager.Instance.GameTime.ToString();

			await winScreenGroup.FadeIn(uiFadeTime);
		}

		public void ShowLoseScreen() => ShowLoseScreenAsync();
		public async Task ShowLoseScreenAsync()
		{
			if (StandardGameManager.Instance.UseScore)
			{	
				int score = StandardGameManager.Instance.GetScore();
				loseScoreAmountText.text = score.ToString();

				loseScoreText.gameObject.SetActive(true);
				loseScoreAmountText.gameObject.SetActive(true);
			}
			else
			{
				loseScoreText.gameObject.SetActive(false);
				loseScoreAmountText.gameObject.SetActive(false);
			}

			//loseTimeText.text = StandardGameManager.Instance.GameTime.ToString();

			await loseScreenGroup.FadeIn(uiFadeTime);
		}

		public void FadePopups() => FadePopupsAsync();
		public async Task FadePopupsAsync()
		{
			List<Task> tasks = new();
			tasks.Add(confirmLoadGroup.FadeOut(uiFadeTime));
			tasks.Add(confirmRestartGroup.FadeOut(uiFadeTime));
			tasks.Add(confirmQuitGroup.FadeOut(uiFadeTime));
			tasks.Add(winScreenGroup.FadeOut(uiFadeTime));
			tasks.Add(loseScreenGroup.FadeOut(uiFadeTime));
			await Task.WhenAll(tasks);
		}

		public void ShowGameStuck()
		{
			gameStuckObj.SetActive(true);
		}
		public void HideGameStuck()
		{
			gameStuckObj.SetActive(false);
		}
	}

}