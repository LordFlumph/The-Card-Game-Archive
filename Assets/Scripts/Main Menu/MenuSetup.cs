namespace CardGameArchive.MainMenu
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UnityEngine.UI;
	using TMPro;

	public class MenuSetup : MonoBehaviour
	{
		[SerializeField] LayoutElement headerDeadzoneElement;
		[SerializeField] LayoutElement footerDeadzoneElement;

		[Header("Main Panel")]
		
		[SerializeField] GameObject mainScrollViewParent;
		[SerializeField] GameObject recentlyAddedCategoryParent;
		[SerializeField] GameObject searchResultsCategoryParent;

		[SerializeField] GameObject recentlyAddedGameOptionPrefab;
		[SerializeField] MenuCategory gameCategoryPrefab;
		[SerializeField] GameObject gameOptionPrefab;

		[Header("Side Panel")]
		[SerializeField] GameVariantQuickButton recentlyPlayedButton;

		[SerializeField] GameObject favouritesHeader;
		[SerializeField] GameObject favouritesButtonParent;
		[SerializeField] GameVariantQuickButton favouritePrefab;

		[Header("Game Info")]
		[SerializeField] List<GameInfo> gameInfo;
		[SerializeField] List<GameInfo> newGameInfo;
		
		[SerializeField] List<GameTerms.GameTag> tagsToUse;

		[Header("Settings Menu")]
		[SerializeField] ToggleSwitch autoMoveToggle;
		[SerializeField] TextMeshProUGUI sfxVolumeText, musicVolumeText, hapticsText;

		async void Start()
		{
			// Didn't start from the Init scene
			if (GameTaskManager.Instance == null)
			{
				SceneManager.LoadScene(0);
				return;
			}

			AdjustDeadzones();			

			SetupMainPanel();
			SetupSidePanel();
			SetupSettings();

			MainMenuManager.Instance.Setup();

			await Awaitable.WaitForSecondsAsync(0.5f);

			LoadingScreen.Instance.Hide();
		}

		void AdjustDeadzones()
		{
			if (Screen.safeArea.yMax != Screen.height)
				headerDeadzoneElement.minHeight = Screen.height - Screen.safeArea.yMax + 25;
			if (Screen.safeArea.yMin > 0)
				footerDeadzoneElement.minHeight = Screen.safeArea.yMin + 25;
		}
		
		void SetupMainPanel()
		{
			// Setup search results category
			// For this, we just have a category that has every game in it, then we disable them all. We only enable options that match the search query
			GenerateGameOptions(gameInfo, searchResultsCategoryParent.transform, true);
			foreach (MenuGameOption gameOption in searchResultsCategoryParent.GetComponentsInChildren<MenuGameOption>())
			{
				gameOption.gameObject.SetActive(false);
			}
			searchResultsCategoryParent.SetActive(false);


			GenerateGameOptions(newGameInfo, recentlyAddedCategoryParent.transform, true);

			foreach (GameTerms.GameTag tag in tagsToUse)
			{
				// No point in generating the category if there aren't any games with the tag
				if (!gameInfo.Any(o => o.Tags.Any(t => t == tag)))
					continue;

				GameTerms.TagInfo tagInfo = GameTerms.GetTagInfo(tag);
				GameObject newCategory = Instantiate(gameCategoryPrefab.gameObject, mainScrollViewParent.transform);
				newCategory.GetComponent<MenuCategory>().TitleText.text = tagInfo.DisplayName.ToUpper();

				GenerateGameOptions(gameInfo.Where(o => o.Tags.Any(t => t == tagInfo.Tag)).ToList(), newCategory.transform, false);
			}

			// Generate All Games category
			GameObject allGamesCategory = Instantiate(gameCategoryPrefab.gameObject, mainScrollViewParent.transform);
			allGamesCategory.GetComponent<MenuCategory>().TitleText.text = "ALL GAMES";

			GenerateGameOptions(gameInfo, allGamesCategory.transform, false);
		}

		void GenerateGameOptions(List<GameInfo> games, Transform parent, bool newGame)
		{
			GameObject prefab = newGame ? recentlyAddedGameOptionPrefab : gameOptionPrefab;

			foreach (GameInfo gameInfo in games)
			{
				GameObject newOption = Instantiate(prefab, parent);
				newOption.GetComponent<MenuGameOption>().Setup(gameInfo);
			}
		}

		void SetupSidePanel()
		{
			// Set recent game
			// Setup favourites

			if (SaveManager.ActiveFile.platformData.lastPlayed != GameTerms.GameVariant.NONE)
			{
				recentlyPlayedButton.Setup(SaveManager.ActiveFile.platformData.lastPlayed);
			}
			else
			{
				recentlyPlayedButton.transform.parent.gameObject.SetActive(false);
			}

			SetupFavourites();
		}

		public void SetupFavourites()
		{
			favouritesButtonParent.transform.DestroyChildren();

			foreach (GameTerms.GameVariant favourite in GameDataManager.Instance.GetFavourites())
			{
				Instantiate(favouritePrefab.gameObject, favouritesButtonParent.transform).GetComponent<GameVariantQuickButton>().Setup(favourite);
			}

			favouritesHeader.SetActive(favouritesButtonParent.transform.childCount != 0);
		}

		void SetupSettings()
		{
			autoMoveToggle.SetValue(SettingsManager.Instance.AutoMoveCards);
			sfxVolumeText.text = SettingsManager.Instance.SFXVolume.ToString();
			musicVolumeText.text = SettingsManager.Instance.MusicVolume.ToString();
			hapticsText.text = SettingsManager.Instance.HapticsStrength.ToString();
		}

		void OnEnable()
		{
			GameDataManager.Instance.OnFavouritesChanged += SetupFavourites;
		}
		void OnDisable()
		{
			GameDataManager.Instance.OnFavouritesChanged -= SetupFavourites;
		}
	}
}