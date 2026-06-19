namespace CardGameArchive.MainMenu
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class MenuSetup : MonoBehaviour
	{
		[SerializeField] LayoutElement headerDeadzoneElement;
		[SerializeField] LayoutElement footerDeadzoneElement;

		[SerializeField] GameObject sideScrollViewParent;
		[SerializeField] GameObject mainScrollViewParent;

		[SerializeField] GameObject newGameCategoryParent;

		[SerializeField] GameObject sidePanelTagButtonPrefab;

		[SerializeField] GameObject newGameOptionPrefab;
		[SerializeField] GameObject gameCategoryPrefab;
		[SerializeField] GameObject gameOptionPrefab;

		[SerializeField] List<GameInfo> gameInfo;
		[SerializeField] List<GameInfo> newGameInfo;

		[SerializeField] List<(GameTerms.GameTag, string)> tagsToUse; 

		void Start()
		{
			if (Screen.safeArea.yMax != Screen.height)
				headerDeadzoneElement.minHeight = Screen.height - Screen.safeArea.yMax + 25;
			if (Screen.safeArea.yMin > 0)
				footerDeadzoneElement.minHeight = Screen.safeArea.yMin + 25;
		}

		void SetupSidePanel()
		{
			// Generate tag buttons above the settings button
		}
		
		void SetupMainPanel()
		{
			// 1. Generate new game options into the new game category
			// 2. Generate categories based on tagsTouse
			// 3. Generate game buttons in each category
		}
	}
}