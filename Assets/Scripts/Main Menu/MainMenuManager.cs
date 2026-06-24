namespace CardGameArchive.MainMenu
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	public class MainMenuManager : MonoBehaviour
    {
        List<MenuGameOption> gameOptions = new();

		EventSystem eventSystem;

		[SerializeField] CanvasGroup mainHeaderGroup, secondaryHeaderGroup;
		
		[SerializeField] CanvasGroup mainMenuGroup, secondaryMenuGroup;

		[SerializeField] List<CanvasGroup> aboutGroup, guideGroup, variantGroup;
		[SerializeField] LayoutElement aboutLayoutElement, guideLayoutElement, variantLayoutElement;

		GameInfo activeGameInfo = null;

        public static MainMenuManager Instance { get; private set;  }

		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		public void Setup()
		{
			if (gameOptions.Count > 0)
				return;

			gameOptions.AddRange(FindObjectsByType<MenuGameOption>(FindObjectsSortMode.None));

			eventSystem = EventSystem.current;

			GameTaskManager.Instance.OnTaskAdded += DisableInput;
			GameTaskManager.Instance.OnTasksFinished += EnableInput;			
		}

		public void GameOptionClicked(MenuGameOption option)
		{
			foreach (var gameOption in gameOptions)
			{
				if (gameOption != option && gameOption.IsDropdownOpen)
					gameOption.ToggleDropdown();
			}
		}

		public void OpenAboutMenu(GameInfo game)
		{
			activeGameInfo = game;
			OpenAboutMenu();
		}
		public void OpenAboutMenu()
		{
			OpenSecondaryMenu();

			aboutLayoutElement.ignoreLayout = false;
			guideLayoutElement.ignoreLayout = true;
			variantLayoutElement.ignoreLayout = true;

			foreach (CanvasGroup group in aboutGroup)
			{
				GameTaskManager.Instance.AddTask(group.FadeIn(0.25f));
			}
			foreach (CanvasGroup group in guideGroup)
			{
				GameTaskManager.Instance.AddTask(group.FadeOut(0.25f));
			}
			foreach (CanvasGroup group in variantGroup)
			{
				GameTaskManager.Instance.AddTask(group.FadeOut(0.25f));
			}
		}
		public void OpenGuideMenu(GameInfo game)
		{
			activeGameInfo = game;
			OpenGuideMenu();
		}
		public void OpenGuideMenu()
		{
			OpenSecondaryMenu();

			aboutLayoutElement.ignoreLayout = true;
			guideLayoutElement.ignoreLayout = false;
			variantLayoutElement.ignoreLayout = true;

			foreach (CanvasGroup group in aboutGroup)
			{
				GameTaskManager.Instance.AddTask(group.FadeOut(0.25f));
			}
			foreach (CanvasGroup group in guideGroup)
			{
				GameTaskManager.Instance.AddTask(group.FadeIn(0.25f));
			}
			foreach (CanvasGroup group in variantGroup)
			{
				GameTaskManager.Instance.AddTask(group.FadeOut(0.25f));
			}
		}
		public void OpenVariantMenu(GameInfo game)
		{
			activeGameInfo = game;
			OpenVariantMenu();
		}
		public void OpenVariantMenu()
		{
			OpenSecondaryMenu();

			aboutLayoutElement.ignoreLayout = true;
			guideLayoutElement.ignoreLayout = true;
			variantLayoutElement.ignoreLayout = false;

			foreach (CanvasGroup group in aboutGroup)
			{
				GameTaskManager.Instance.AddTask(group.FadeOut(0.25f));
			}
			foreach (CanvasGroup group in guideGroup)
			{
				GameTaskManager.Instance.AddTask(group.FadeOut(0.25f));
			}
			foreach (CanvasGroup group in variantGroup)
			{
				GameTaskManager.Instance.AddTask(group.FadeIn(0.25f));
			}
		}
		public void OpenMainMenu()
		{
			GameTaskManager.Instance.AddTask(mainMenuGroup.FadeIn(0.25f));
			GameTaskManager.Instance.AddTask(mainHeaderGroup.FadeIn(0.25f));
			GameTaskManager.Instance.AddTask(secondaryMenuGroup.FadeOut(0.25f));
			GameTaskManager.Instance.AddTask(secondaryHeaderGroup.FadeOut(0.25f));
		}

		void OpenSecondaryMenu()
		{
			GameTaskManager.Instance.AddTask(mainMenuGroup.FadeOut(0.25f));
			GameTaskManager.Instance.AddTask(mainHeaderGroup.FadeOut(0.25f));
			GameTaskManager.Instance.AddTask(secondaryMenuGroup.FadeIn(0.25f));
			GameTaskManager.Instance.AddTask(secondaryHeaderGroup.FadeIn(0.25f));
		}

		public void EnableInput() => eventSystem.enabled = true;
		public void DisableInput() => eventSystem.enabled = false;

		private void OnDisable()
		{
			GameTaskManager.Instance.OnTaskAdded -= DisableInput;
			GameTaskManager.Instance.OnTasksFinished -= EnableInput;
		}
	}

}