namespace CardGameArchive
{
	using UnityEngine;
	using UnityEngine.Pool;

	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }

		[SerializeField] AudioSource cardMoveSource;

		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public void OnCardMove(GameBoard.CardMoveEvent eventData)
		{
			if (!eventData.teleport)
				cardMoveSource.PlayOneShot(cardMoveSource.clip);
		}
	}

}