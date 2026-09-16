namespace CardGameArchive
{
	using UnityEngine;
	using UnityEngine.Audio;

	/// <summary>
	/// Manages all code related to audio
	/// </summary>
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }

		[SerializeField] AudioMixer mixer;
		[SerializeField] AudioSource bgmSource;
		[SerializeField] AudioSource cardMoveSource;
		[SerializeField] AudioSource invalidActionSource;

		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
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
	
		public void SetMusicVolume(int volume)
		{
			mixer.SetFloat("musicVolume", Mathf.Log10(Mathf.Clamp(volume/10f, 0.0001f, 10f)) * 20);
		}

		public void SetSFXVolume(int volume)
		{
			mixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Clamp(volume/10f, 0.0001f, 10f)) * 20);
		}
	}

}