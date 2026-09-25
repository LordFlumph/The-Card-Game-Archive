namespace CardGameArchive.MainMenu
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using TMPro;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;
	using UnityEngine.UIElements;

	public class MainMenuManager : MonoBehaviour
    {
        List<MenuGameOption> gameOptions = new();

		EventSystem eventSystem;

		[SerializeField] GameObject interactionBlocker;
		[SerializeField] GameObject searchCategoryParent;
		List<MenuGameOption> searchGameOptions = new();

		[SerializeField] CanvasGroup mainHeaderGroup, secondaryHeaderGroup;
		
		[SerializeField] CanvasGroup mainMenuGroup, secondaryMenuGroup;

		[Header("Secondary Menu")]
		[SerializeField] List<CanvasGroup> aboutGroup;
		[SerializeField] List<CanvasGroup> guideGroup, variantGroup;
		[SerializeField] LayoutElement aboutLayoutElement, guideLayoutElement, variantLayoutElement;

		[SerializeField] TextMeshProUGUI gameTitleText;

		[SerializeField] TextMeshProUGUI aboutText;
		[SerializeField] Transform guideObjParent;

		[SerializeField] ScrollRect aboutScrollRect, guideScrollRect, variantScrollRect;

		[SerializeField] GameVariantButton variantObjectPrefab;
		[SerializeField] Transform variantObjectParent;

		[Header("Settings Menu")]
		[SerializeField] CanvasGroup settingsGroup;

        public static MainMenuManager Instance { get; private set;  }

		float backButtonQuitTimer = 0;

		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		void Update()
		{
			if (backButtonQuitTimer > 0)
			{
				backButtonQuitTimer -= Time.deltaTime;
			}
		}

		public void Setup()
		{
			if (gameOptions.Count > 0)
				return;

			searchGameOptions.AddRange(searchCategoryParent.GetComponentsInChildren<MenuGameOption>(true));

			gameOptions.AddRange(FindObjectsByType<MenuGameOption>(FindObjectsInactive.Include, FindObjectsSortMode.None));

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

		public void OpenAboutMenu(GameInfo game) { SetupSecondaryMenu(game); OpenAboutMenu(); }
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

		public void OpenGuideMenu(GameInfo game) { SetupSecondaryMenu(game); OpenGuideMenu(); }
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

		public void OpenVariantMenu(GameInfo game) { SetupSecondaryMenu(game); OpenVariantMenu(); }
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
			GameTaskManager.Instance.AddTask(settingsGroup.FadeOut(0.25f));
			GameTaskManager.Instance.QueueTask(() => 
			{
				aboutScrollRect.verticalNormalizedPosition = 1f;
				guideScrollRect.verticalNormalizedPosition = 1f;
				variantScrollRect.verticalNormalizedPosition = 1f;
			});
		}
		void OpenSecondaryMenu()
		{
			GameTaskManager.Instance.AddTask(mainMenuGroup.FadeOut(0.25f));
			GameTaskManager.Instance.AddTask(mainHeaderGroup.FadeOut(0.25f));
			GameTaskManager.Instance.AddTask(secondaryMenuGroup.FadeIn(0.25f));
			GameTaskManager.Instance.AddTask(secondaryHeaderGroup.FadeIn(0.25f));
			GameTaskManager.Instance.AddTask(settingsGroup.FadeOut(0.25f));
		}

		public void SetupSecondaryMenu(GameInfo gameInfo)
		{
			gameTitleText.text = gameInfo.DisplayName;
			aboutText.text = gameInfo.AboutText;

			guideObjParent.DestroyChildren();
			if (gameInfo.GuideObject != null)
				Instantiate(gameInfo.GuideObject, guideObjParent);
			else
				Debug.LogWarning("Guide object is null for game: " + gameInfo.DisplayName);

			variantObjectParent.DestroyChildren();

			foreach (GameInfo.GameVariantInfo variant in gameInfo.Variants)
			{
				GameVariantButton variantButton = Instantiate(variantObjectPrefab.gameObject, variantObjectParent).GetComponent<GameVariantButton>();
				variantButton.Setup(variant);
			}
		}

		public void OpenSettingsMenu()
		{
			GameTaskManager.Instance.AddTask(mainMenuGroup.FadeOut(0.25f));
			GameTaskManager.Instance.AddTask(mainHeaderGroup.FadeOut(0.25f));
			GameTaskManager.Instance.AddTask(secondaryMenuGroup.FadeOut(0.25f));
			GameTaskManager.Instance.AddTask(secondaryHeaderGroup.FadeIn(0.25f));
			GameTaskManager.Instance.AddTask(settingsGroup.FadeIn(0.25f));
		}

		public async void StartGame(GameTerms.GameVariant variant)
		{
			LoadingScreen.Instance.Show();
			await GameTaskManager.Instance.WhenAll();
			GameSceneManager.Instance.OpenGame(variant);
		}

		public void OnSearchBarChanged(string newSearch)
		{
			if (string.IsNullOrEmpty(newSearch))
			{
				searchCategoryParent.SetActive(false);
				return;
			}

			bool searchFound = false;
			string lowerSearch = newSearch.ToLower();
			foreach (MenuGameOption option in searchGameOptions)
			{
				if (option.gameInfo.Name.ToString().ToLower().Contains(lowerSearch) ||
					option.gameInfo.DisplayName.ToLower().Contains(lowerSearch) ||
					option.gameInfo.Tags.Any(o => o.ToString().ToLower().Contains(lowerSearch)))
				{
					option.gameObject.SetActive(true);
					searchFound = true;
				}
				else
				{
					option.gameObject.SetActive(false);
				}
			}

			searchCategoryParent.SetActive(searchFound);
		}

		public void ShowConsentForm()
		{
			AdManager.Instance.OpenPrivacyOptions();
		}

		public void EnableInput() => interactionBlocker.SetActive(false);
		public void DisableInput() => interactionBlocker.SetActive(true);

		public void BackButtonPressed()
		{
			if (secondaryMenuGroup.alpha > 0 || settingsGroup.alpha > 0)
			{
				OpenMainMenu();
			}
			else
			{
				if (gameOptions.Any(o => o.IsDropdownOpen))
				{
					GameOptionClicked(null);
				}
				else
				{
					if (backButtonQuitTimer > 0)
					{
#if !UNITY_EDITOR
						Application.Quit(); 
#else
						UnityEditor.EditorApplication.isPlaying = false;
#endif
					}
					else
					{
						backButtonQuitTimer = 2f;
						ToastService.Show("Press back again to quit"); 
					}
				}
			}
		}

		private void OnDisable()
		{
			if (GameTaskManager.Instance == null)
				return;

			GameTaskManager.Instance.OnTaskAdded -= DisableInput;
			GameTaskManager.Instance.OnTasksFinished -= EnableInput;
		}
	}

}