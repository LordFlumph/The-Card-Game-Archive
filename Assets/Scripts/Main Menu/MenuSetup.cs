namespace CardGameArchive.MainMenu
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;

	public class MenuSetup : MonoBehaviour
	{
		[SerializeField] LayoutElement headerDeadzoneElement;
		[SerializeField] LayoutElement footerDeadzoneElement;

		[SerializeField] GameObject sideScrollViewParent;
		[SerializeField] GameObject mainScrollViewParent;

		[SerializeField] GameObject recentlyAddedCategoryParent;

		[SerializeField] GameObject sidePanelTagButtonPrefab;

		[SerializeField] GameObject recentlyAddedGameOptionPrefab;
		[SerializeField] MenuCategory gameCategoryPrefab;
		[SerializeField] GameObject gameOptionPrefab;

		[SerializeField] List<GameInfo> gameInfo;
		[SerializeField] List<GameInfo> newGameInfo;

		[System.Serializable] 
		public struct TagInfo 
		{
			public GameTerms.GameTag Tag; 
			[SerializeField] string displayName; 
			public string DisplayName { get { return displayName.Length > 0 ? displayName : Tag.ToString(); } } 
		}
		
		[SerializeField] List<TagInfo> tagsToUse;

		void Start()
		{
			AdjustDeadzones();			

			SetupMainPanel();
			SetupSidePanel();
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
			GenerateGameOptions(newGameInfo, recentlyAddedCategoryParent.transform, true);

			foreach (TagInfo tagInfo in tagsToUse)
			{
				// No point in generating the category if there aren't any games with the tag
				if (!gameInfo.Any(o => o.Tags.Any(t => t == tagInfo.Tag)))
					continue;

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
			// Generate tag buttons above the settings button
		}
	}
}