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

		[SerializeField] Button undoButton, cheatButton;

		[SerializeField] GameObject gameStuckObj, cheatActiveObj;

		[SerializeField] GraphicRaycaster uiRaycaster;

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
			{
				if (undoButton != null)
					undoButton.interactable = StandardGameManager.Instance.CanUndo;
				
				if (cheatButton != null)
					cheatButton.interactable = AdManager.Instance.RewardAdReady;
			}
		}

		public void EnableUI()
		{
			uiRaycaster.enabled = true;
		}
		public void DisableUI()
		{
			uiRaycaster.enabled = false;
		}

		public void ShowRestartConfirmation()
		{
			PopupMenuManager.Instance.ShowRestartConfirmation();
		}

		public void ShowQuitConfirmation()
		{
			PopupMenuManager.Instance.ShowQuitConfirmation();
		}


		public void Restart()
		{
			DisableUI();
			StandardGameManager.Instance.RestartGame();
		}
		public void Undo()
		{
			StandardGameManager.Instance.UndoMove();
		}
		public void Cheat()
		{
			StandardGameManager.Instance.ActivateCheat();
		}
		public void Quit() => QuitAsync();

		async Task QuitAsync()
		{
			DisableUI();
			LoadingScreen.Instance.Show();
			await GameTaskManager.Instance.WhenAll();
			GameSceneManager.Instance.OpenMainMenu();
		}

		public void ShowGameStuck()
		{
			gameStuckObj.SetActive(true);
		}
		public void HideGameStuck()
		{
			gameStuckObj.SetActive(false);
		}

		public void ShowCheatActive()
		{
			cheatButton.interactable = false;
			cheatActiveObj.SetActive(true);
		}
		public void HideCheatActive()
		{
			cheatButton.interactable = true;
			cheatActiveObj.SetActive(false);
		}
	}

}