namespace CardGameArchive.Old
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;
	using UnityEngine.UI;

	public class MainMenuManager : MonoBehaviour
	{
		public static MainMenuManager Instance { get; private set; }

		[SerializeField] CanvasGroup mainCanvasGroup;

		[SerializeField] LayoutElement headerDeadzoneElement;
		[SerializeField] LayoutElement footerDeadzoneElement;

		[SerializeField] List<MenuGamePopup> gamePopups = new();
		[SerializeField] List<MenuGameButton> gameButtons = new();
		[SerializeField] GameObject popupParent;

		[SerializeField] RectTransform settingsTransform;

		[SerializeField] GameObject settingsOpenButton, settingsCloseButton;

		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		void Start()
		{
			if (Screen.safeArea.yMax != Screen.height)
				headerDeadzoneElement.minHeight = Screen.height - Screen.safeArea.yMax + 25;
			if (Screen.safeArea.yMin > 0)
				footerDeadzoneElement.minHeight = Screen.safeArea.yMin + 25;

			gamePopups.AddRange(FindObjectsByType<MenuGamePopup>(FindObjectsInactive.Include, FindObjectsSortMode.None));
			gameButtons.AddRange(FindObjectsByType<MenuGameButton>(FindObjectsInactive.Include, FindObjectsSortMode.None));
		}

		public void OnGamePressed(GameTerms.GameName gameName)
		{
			MenuGamePopup popup = gamePopups.Find(p => p.gameName == gameName);
			MenuGameButton button = gameButtons.Find(b => b.gameName == gameName);

			// Just to be safe
			ClosePopups();

			if (popup != null && button != null)
			{
				popup.Open(button.transform.position);
				popupParent.SetActive(true);
			}
			else
			{
				Debug.LogError($"Popup or button not found for game: {gameName}");
			}

		}

		public void ClosePopups()
		{
			foreach (MenuGamePopup popup in gamePopups)
			{
				popup.Close();
			}
			popupParent.SetActive(false);
		}

		public void EnableUI()
		{
			mainCanvasGroup.interactable = true;
		}
		public void DisableUI()
		{
			mainCanvasGroup.interactable = false;
		}

		public void ToggleSettings()
		{
			ClosePopups();

			if (GameTaskManager.Instance.TaskCount == 0)
			{
				GameTaskManager.Instance.AddTask(MoveSettingsPanel(settingsTransform.pivot.x == 1));
				settingsOpenButton.SetActive(!settingsOpenButton.activeSelf);
				settingsCloseButton.SetActive(!settingsCloseButton.activeSelf);
			}
				
		}

		async Task MoveSettingsPanel(bool open)
		{
			Vector2 targetPivot = new(open ? 0 : 1, settingsTransform.pivot.y);
			Vector2 initialPivot = settingsTransform.pivot;

			float t = 0;

			while (Vector2.Distance(settingsTransform.pivot, targetPivot) > 0.01f)
			{
				settingsTransform.pivot = Vector2.Lerp(initialPivot, targetPivot, t);
				t += Time.deltaTime * 5;
				await Awaitable.EndOfFrameAsync();
			}
			settingsTransform.pivot = targetPivot;
		}


		private void OnEnable()
		{
			GameTaskManager.Instance.OnTaskAdded += DisableUI;
			GameTaskManager.Instance.OnTasksFinished += EnableUI;
		}
		private void OnDisable()
		{
			GameTaskManager.Instance.OnTaskAdded -= DisableUI;
			GameTaskManager.Instance.OnTasksFinished -= EnableUI;
		}

	}

}