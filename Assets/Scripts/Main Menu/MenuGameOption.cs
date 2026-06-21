namespace CardGameArchive.MainMenu
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public class MenuGameOption : MonoBehaviour
	{
		GameInfo gameInfo;

		[SerializeField] TextMeshProUGUI nameText, aboutText;

		[SerializeField] Button aboutButton, guideButton, playButton;

		[SerializeField] GameObject variantParent;

		CancellationTokenSource dropdownCT;

		public void Setup(GameInfo info)
		{
			gameInfo = info;

			nameText.text = info.DisplayName;

			aboutText.text = info.AboutText;
			string truncatedText = aboutText.GetTruncatedText();
			if (aboutText.text != truncatedText)
			{
				aboutText.text = truncatedText.Substring(0, truncatedText.Length - 5) + "...";
			}
		}

		public void OpenDropdown()
		{
			GameTaskManager.Instance.AddTask(OpenDropdownAsync());
		}

		public async Task OpenDropdownAsync()
		{
			ResetDropdownCT();
			CancellationToken token = dropdownCT.Token;
			try
			{
				await Task.Yield();
				token.ThrowIfCancellationRequested();
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
			}
		}

		public void CloseDropdown()
		{
			GameTaskManager.Instance.AddTask(CloseDropdownAsync());
		}

		public async Task CloseDropdownAsync()
		{
			ResetDropdownCT();
			CancellationToken token = dropdownCT.Token;
			try
			{
				await Task.Yield();
				token.ThrowIfCancellationRequested();
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
			}
		}

		void ResetDropdownCT()
		{
			dropdownCT?.Cancel();
			dropdownCT?.Dispose();
			dropdownCT = new CancellationTokenSource();
		}

		public void AboutButtonPressed()
		{

		}

		public void GuideButtonPressed()
		{

		}

		public void PlayButtonPressed()
		{
			if (gameInfo.Variants.Count > 1)
			{
				ToggleVariantDropdown();
			}
			else
			{
				// Start game
			}
		}

		public void ToggleVariantDropdown()
		{

		}
		
		
		void OnDestroy()
		{
			dropdownCT?.Cancel();
			dropdownCT?.Dispose();
		}
	}

}
