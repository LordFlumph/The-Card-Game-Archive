namespace CardGameArchive.MainMenu
{
    using TMPro;
    using UnityEngine;

	public class SettingsMenuManager : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI sfxText, musicText;

		public void SetAutoMove(bool Value)
		{
			SettingsManager.Instance.AutoMoveCards = Value;
		}

		public void ModifySFXVolume(int Value)
		{
			SettingsManager.Instance.SFXVolume = Mathf.Clamp(SettingsManager.Instance.SFXVolume + Value, 0, 10);
			sfxText.text = SettingsManager.Instance.SFXVolume.ToString();
		}

		public void ModifyMusicVolume(int Value)
		{
			SettingsManager.Instance.MusicVolume = Mathf.Clamp(SettingsManager.Instance.MusicVolume + Value, 0, 10);
			musicText.text = SettingsManager.Instance.MusicVolume.ToString();
		}
	}

}