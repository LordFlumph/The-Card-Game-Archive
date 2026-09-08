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

		[SerializeField] GameObject gameStuckObj;

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
	}

}