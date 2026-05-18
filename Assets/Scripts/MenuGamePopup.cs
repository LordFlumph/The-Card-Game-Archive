namespace CardGameArchive
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
	using UnityEngine.Events;

	public class MenuGamePopup : MonoBehaviour
    {
        [SerializeField] Button playButton;


        public void Setup(GameTerms.GameName gameName)
        {
            // Setup Play button to load the scene
            playButton.onClick.AddListener(new UnityAction(async () =>
            {
                LoadingScreen.Instance.Show();
                MainMenuManager.Instance.DisableUI();
				await GameTaskManager.Instance.WhenAll();
                GameSceneManager.Instance.OpenGame(gameName);
            }));
            // Setup How To Play button and popup


		}
	}

}