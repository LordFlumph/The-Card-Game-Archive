namespace CardGameArchive.MainMenu
{
	using System.Linq;
	using TMPro;
	using UnityEngine;

	public class GameVariantQuickButton : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI gameText, variantText;

		public GameTerms.GameVariant Variant { get; private set; }

		public void Setup(GameTerms.GameVariant variant)
		{
			Variant = variant;

			GameInfo info = GameDataManager.Instance.GetGameInfo(variant);
			gameText.text = info.DisplayName;
			variantText.text = info.Variants.First(o => o.Variant == variant).DisplayName;
		}

		public void OnClick()
		{
			MainMenuManager.Instance.StartGame(Variant);
		}
	}

}