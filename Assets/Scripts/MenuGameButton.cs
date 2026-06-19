namespace CardGameArchive.Old
{
	using UnityEngine;
	using UnityEngine.UI;

	public class MenuGameButton : MonoBehaviour
	{
		Button button;
		[field: SerializeField] public GameTerms.GameName gameName { get; private set; }

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