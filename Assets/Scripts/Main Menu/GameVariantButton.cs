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

		public GameTerms.GameVariant Variant {get; private set;}

		public void Setup(GameInfo.GameVariantInfo info)
		{
			titleText.text = info.DisplayName;
			descriptionText.text = info.Description;
			icon.sprite = info.Icon;

			Variant = info.Variant;

			// isFavourite = MainMenuManager.Instance.IsFavourite(variant);
			// if (MainMenuManager.Instance.IsFavourite(variant))
			// 	favouriteButton.ToggleFavourite();
		}

		public void OnClick()
		{
			MainMenuManager.Instance.StartGame(Variant);
		}
	}

}