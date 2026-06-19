namespace CardGameArchive.MainMenu
{
	using System.Threading.Tasks;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public class MenuGameOption : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI nameText, aboutText;

		[SerializeField] Button aboutButton, guideButton, playButton;

		bool dropdownMoving;

		public void Setup(GameInfo info)
		{
			nameText.text = info.DisplayName;

			aboutText.text = info.AboutText;
			string truncatedText = aboutText.GetTruncatedText();
			if (aboutText.text != truncatedText)
			{
				aboutText.text = truncatedText.Substring(0, truncatedText.Length - 5) + "...";
			}
		}

		public async Task OpenDropdown()
		{
			if (dropdownMoving)
				return;

			dropdownMoving = true;
		}

		public async Task CloseDropdown()
		{

		}
	}

}