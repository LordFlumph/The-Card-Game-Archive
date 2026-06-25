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
		public GameInfo gameInfo { get; private set; }

		Animator animator;

		[SerializeField] Button mainButton;
		[SerializeField] TextMeshProUGUI nameText, aboutText;
		[SerializeField] Image icon;
		[SerializeField] Color iconColour;

		public bool IsDropdownOpen { get; private set; }

		bool animating;

		void Awake()
		{
			animator = GetComponent<Animator>();
		}

		public async void Setup(GameInfo info)
		{
			gameInfo = info;

			nameText.text = info.DisplayName;
			icon.sprite = info.Icon;

			aboutText.text = info.AboutText;
			await Awaitable.EndOfFrameAsync();
			string truncatedText = aboutText.GetTruncatedText();
			if (aboutText.text != truncatedText)
			{
				//aboutText.text = truncatedText.Substring(0, truncatedText.Length - 20) + "...";
			}
		}

		public void ToggleDropdown()
		{
			if (animating)
				return;

			animator.Play(IsDropdownOpen ? "CloseDropdown" : "OpenDropdown");
			animating = true;
			GameTaskManager.Instance.AddTask(async () => { while (animating) await Task.Yield(); });

			if (!IsDropdownOpen)
			{
				MainMenuManager.Instance.GameOptionClicked(this);
			}
		}

		public void DropdownAnimationFinished()
		{
			IsDropdownOpen = !IsDropdownOpen;
			animating = false;
		}

		public void AboutButtonPressed()
		{
			MainMenuManager.Instance.OpenAboutMenu(gameInfo);
		}

		public void GuideButtonPressed()
		{
			MainMenuManager.Instance.OpenGuideMenu(gameInfo);
		}

		public void PlayButtonPressed()
		{
			MainMenuManager.Instance.OpenVariantMenu(gameInfo);
		}
	}

}
