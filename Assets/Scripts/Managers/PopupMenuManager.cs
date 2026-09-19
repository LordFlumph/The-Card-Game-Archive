namespace CardGameArchive
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using TMPro;
	using UnityEngine;

	public class PopupMenuManager : MonoBehaviour
	{
		public static PopupMenuManager Instance { get; private set; }

		[SerializeField] CanvasGroup winScreenGroup;
		[SerializeField] TextMeshProUGUI winScoreText, winScoreAmountText;
		[SerializeField] TextMeshProUGUI winTimeText;

		[SerializeField] CanvasGroup loseScreenGroup;
		[SerializeField] TextMeshProUGUI loseScoreText, loseScoreAmountText;
		[SerializeField] TextMeshProUGUI loseTimeText;

		[SerializeField] CanvasGroup confirmLoadGroup, confirmRestartGroup, confirmQuitGroup;

		[SerializeField] float uiFadeTime = 0.2f;

		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		public void RestartGame()
		{
			FadePopupsAsync();
			UIManager.Instance.Restart();
		}

		public void Quit()
		{
			FadePopupsAsync();
			UIManager.Instance.Quit();
		}

		public void FadePopups() => FadePopupsAsync();
		public async Task FadePopupsAsync()
		{
			List<Task> tasks = new();
			tasks.Add(confirmLoadGroup.FadeOut(uiFadeTime));
			tasks.Add(confirmRestartGroup.FadeOut(uiFadeTime));
			tasks.Add(confirmQuitGroup.FadeOut(uiFadeTime));
			tasks.Add(winScreenGroup.FadeOut(uiFadeTime));
			tasks.Add(loseScreenGroup.FadeOut(uiFadeTime));
			await Task.WhenAll(tasks);
		}

		public void ShowLoadConfirmation() => ShowLoadConfirmationAsync();
		public async Task ShowLoadConfirmationAsync()
		{
			await confirmLoadGroup.FadeIn(uiFadeTime);
		}

		public void ShowRestartConfirmation() => ShowRestartConfirmationAsync();
		public async Task ShowRestartConfirmationAsync()
		{
			await confirmRestartGroup.FadeIn(uiFadeTime);
		}
		public void ShowQuitConfirmation() => ShowQuitConfirmationAsync();
		public async Task ShowQuitConfirmationAsync()
		{
			await confirmQuitGroup.FadeIn(uiFadeTime);
		}

		public void ShowWinScreen() => ShowWinScreenAsync();
		public async Task ShowWinScreenAsync()
		{
			if (StandardGameManager.Instance.UseScore)
			{
				int score = StandardGameManager.Instance.GetScore();
				winScoreAmountText.text = score.ToString();

				winScoreText.gameObject.SetActive(true);
				winScoreAmountText.gameObject.SetActive(true);
			}
			else
			{
				//winScoreText.gameObject.SetActive(false);
				//winScoreAmountText.gameObject.SetActive(false);
			}

			//winTimeText.text = StandardGameManager.Instance.GameTime.ToString();

			await winScreenGroup.FadeIn(uiFadeTime);
		}

		public void ShowLoseScreen() => ShowLoseScreenAsync();
		public async Task ShowLoseScreenAsync()
		{
			if (StandardGameManager.Instance.UseScore)
			{
				int score = StandardGameManager.Instance.GetScore();
				loseScoreAmountText.text = score.ToString();

				loseScoreText.gameObject.SetActive(true);
				loseScoreAmountText.gameObject.SetActive(true);
			}
			else
			{
				//loseScoreText.gameObject.SetActive(false);
				//loseScoreAmountText.gameObject.SetActive(false);
			}

			//loseTimeText.text = StandardGameManager.Instance.GameTime.ToString();

			await loseScreenGroup.FadeIn(uiFadeTime);
		}
	} 
}
