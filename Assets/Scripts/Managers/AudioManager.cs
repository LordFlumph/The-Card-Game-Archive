namespace CardGameArchive
{
	using UnityEngine;

	/// <summary>
	/// Manages all code related to audio
	/// </summary>
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }

		[SerializeField] AudioSource cardMoveSource;
		[SerializeField] AudioSource invalidActionSource;

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

		public void OnInvalidAction(Card card)
		{
			invalidActionSource.PlayOneShot(invalidActionSource.clip);
		}
	}

}