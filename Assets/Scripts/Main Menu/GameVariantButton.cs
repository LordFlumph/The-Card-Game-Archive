namespace CardGameArchive.MainMenu
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameVariantButton : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI titleText, descriptionText;
		[SerializeField] Image icon;

		[SerializeField] FavouriteButton favouriteButton;

		GameTerms.GameVariant variant;

		public void Setup(GameInfo.GameVariantInfo info)
		{
			titleText.text = info.DisplayName;
			descriptionText.text = info.Description;
			icon.sprite = info.Icon;

			variant = info.Variant;

			// isFavourite = MainMenuManager.Instance.IsFavourite(variant);
			// if (MainMenuManager.Instance.IsFavourite(variant))
			// 	favouriteButton.ToggleFavourite();
		}

		public void OnClick()
		{
			MainMenuManager.Instance.StartGame(variant);
		}

		public void OnFavouriteClick()
		{
			// MainMenuManager.Instance.ToggleFavourite(variant);
		}
	}

}