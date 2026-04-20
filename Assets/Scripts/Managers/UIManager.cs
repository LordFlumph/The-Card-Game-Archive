namespace CardGameArchive
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Handles all code related to managing the UI
	/// </summary>
	public class UIManager : MonoBehaviour
	{
		public static UIManager Instance { get; private set; }

		[SerializeField] Button restartButton;
		[SerializeField] Button undoButton;

		[SerializeField] CanvasGroup winScreenGroup, loseScreenGroup;
		[SerializeField] CanvasGroup confirmLoadGroup, confirmRestartGroup;

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
			undoButton.interactable = BaseGameManager.Instance.CanUndo;
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
			restartButton.interactable = false;
			undoButton.interactable = false;
			BaseGameManager.Instance.RestartGame();
		}
		public void Undo()
		{
			BaseGameManager.Instance.UndoMove();
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

		public void ShowWinScreen() => ShowWinScreenAsync();
		public async Task ShowWinScreenAsync()
		{
			await winScreenGroup.FadeIn(uiFadeTime);
		}

		public void ShowLoseScreen() => ShowLoseScreenAsync();
		public async Task ShowLoseScreenAsync()
		{
			await loseScreenGroup.FadeIn(uiFadeTime);
		}

		public void FadePopups() => FadePopupsAsync();
		public async Task FadePopupsAsync()
		{
			List<Task> tasks = new();
			tasks.Add(confirmLoadGroup.FadeOut(uiFadeTime));
			tasks.Add(confirmRestartGroup.FadeOut(uiFadeTime));
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

		public void Quit()
		{
			// Load main menu
		}

	}

}