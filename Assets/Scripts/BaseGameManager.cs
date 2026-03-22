namespace CardGameArchive
{
	using UnityEngine;

	public abstract class BaseGameManager : MonoBehaviour
	{
		public static BaseGameManager Instance { get; private set; }

		protected Deck Deck { get; } = new Deck();

		[SerializeField] protected GameBoard gameBoard;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		protected virtual void Start()
		{
			StartGame();
		}

		public abstract void StartGame();
		public abstract void OnDeckClicked(Deck deck);
		public abstract void OnCardClicked(Card card);
		public abstract void OnCardDropped(Card card);
	}
}
