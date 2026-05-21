namespace CardGameArchive
{
	using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
	using UnityEngine.Events;

	public class MenuGamePopup : MonoBehaviour
    {
        [field: SerializeField] public GameTerms.GameName gameName { get; private set; }
		[SerializeField] List<GameTerms.GameName> gameVariants;
		[SerializeField] GameObject variantPopup, howToPlayPopup, historyPopup;

		public void Open(Vector3 buttonPosition)
		{
			// Later, make it expand but for now, just make it appear
			gameObject.SetActive(true);

			Vector3 newPos = Vector3.zero;
			newPos.y = Mathf.Clamp(buttonPosition.y, 0, Screen.height - GetComponent<RectTransform>().rect.height);
		}
		public void Close()
		{
			// Later, make it contract but for now, just make it disappear
			gameObject.SetActive(false);

			if (variantPopup != null)
				variantPopup.SetActive(false);

			if (howToPlayPopup != null)
				howToPlayPopup.SetActive(false);

			if (historyPopup != null)
				historyPopup.SetActive(false);
		}

		public void OnPlay()
        {
			if (variantPopup != null)
			{
				variantPopup.SetActive(!variantPopup.activeSelf);
			}
			else
			{
				OpenGame(gameName);
			}			
		}
		public void OnPlayVariant(int variantIndex)
		{
			if (gameVariants.Count > variantIndex && variantIndex >= 0)
				OpenGame(gameVariants[variantIndex]);
		}

		async void OpenGame(GameTerms.GameName gameName)
		{
			LoadingScreen.Instance.Show();
			await GameTaskManager.Instance.WhenAll();
			GameSceneManager.Instance.OpenGame(gameName);
		}
	}

}