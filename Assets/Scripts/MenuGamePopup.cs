namespace CardGameArchive
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
	using UnityEngine.Events;

	public class MenuGamePopup : MonoBehaviour
    {
        [field: SerializeField] public GameTerms.GameName gameName { get; private set; }

		public void Open()
		{
			// Later, make it expand but for now, just make it appear
			gameObject.SetActive(true);
		}
		public void Close()
		{
			// Later, make it contract but for now, just make it disappear
			gameObject.SetActive(false);
		}

		public void OnPlay()
        {
			LoadingScreen.Instance.Show();
			MainMenuManager.Instance.DisableUI();
			// Queue it so that it triggers when everything else is finished
			GameTaskManager.Instance.QueueTask(() => GameSceneManager.Instance.OpenGame(gameName));
		}
	}

}