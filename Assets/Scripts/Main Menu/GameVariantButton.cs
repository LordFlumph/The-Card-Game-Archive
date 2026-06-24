namespace CardGameArchive.MainMenu
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameVariantButton : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI titleText, descriptionText;
		[SerializeField] Image icon;

		GameTerms.GameVariant variant;

		public void Setup(GameInfo.GameVariantInfo info)
		{
			titleText.text = info.DisplayName;
			descriptionText.text = info.Description;
			icon.sprite = info.Icon;

			variant = info.Variant;
		}

		public void OnClick()
		{
			MainMenuManager.Instance.StartGame(variant);
		}
	}

}