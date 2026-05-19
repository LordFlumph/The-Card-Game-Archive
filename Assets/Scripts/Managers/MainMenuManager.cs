namespace CardGameArchive
{
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.UI;

    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance { get; private set; }

		[SerializeField] GraphicRaycaster uiRaycaster;

        [SerializeField] LayoutElement headerDeadzoneElement;
        [SerializeField] LayoutElement footerDeadzoneElement;

        [SerializeField] List<MenuGamePopup> gamePopups = new();
        [SerializeField] GameObject popupParent;

		void Awake()
		{
            if (Instance != null)
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
		}

        public void OnGamePressed(GameTerms.GameName gameName)
        {
            MenuGamePopup popup = gamePopups.Find(p => p.gameName == gameName);

            if (popup != null)
            {
                popup.Open();
            }
			popupParent.SetActive(true);
		}

        public void ClosePopups()
        {
			foreach (MenuGamePopup popup in gamePopups)
			{
				popup.Close();
			}
			popupParent.SetActive(false);
		}

        public void DisableUI()
        {
            uiRaycaster.enabled = false;
        }
    }

}