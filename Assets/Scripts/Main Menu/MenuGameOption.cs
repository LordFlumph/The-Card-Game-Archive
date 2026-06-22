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

		Animator animator;

		[SerializeField] TextMeshProUGUI nameText, aboutText;

		[SerializeField] Button aboutButton, guideButton, playButton;

		public bool IsDropdownOpen { get; private set; }
		public bool IsVariantDropdownOpen { get; private set; }

		bool animating;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}
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

		public void ToggleDropdown()
		{
			if (animating)
				return;

			animator.Play(IsDropdownOpen ? "CloseDropdown" : "OpenDropdown");
			animating = true;
			GameTaskManager.Instance.AddTask(async () => { while (animating) await Task.Yield(); });
		}

		public void DropdownAnimationFinished()
		{
			IsDropdownOpen = !IsDropdownOpen;
			animating = false;
		}

		public void AboutButtonPressed()
		{

		}

		public void GuideButtonPressed()
		{

		}

		public void PlayButtonPressed()
		{

		}
	}

}
