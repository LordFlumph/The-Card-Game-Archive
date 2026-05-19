namespace CardGameArchive
{
	using UnityEngine;
	using UnityEngine.UI;

	public class MenuGameButton : MonoBehaviour
	{
		Button button;
		[SerializeField] GameTerms.GameName gameName;

		void Awake()
		{
			button = GetComponent<Button>();
		}

		void Start()
		{
			button.onClick.AddListener(() => MainMenuManager.Instance.OnGamePressed(gameName));
		}
	}
}