namespace CardGameArchive
{
	using System;
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

		[SerializeField] CanvasGroup winScreenGroup;

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
			restartButton.interactable = false;
			undoButton.interactable = false;
			BaseGameManager.Instance.RestartGame();
		}
		public void Undo()
		{
			BaseGameManager.Instance.UndoMove();
		}

		public void ShowWinScreen()
		{
			winScreenGroup.FadeIn(0.2f);
		}
	}

}