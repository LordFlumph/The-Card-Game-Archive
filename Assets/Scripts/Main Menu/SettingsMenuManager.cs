namespace CardGameArchive.MainMenu
{
	using UnityEngine;

	public class SettingsMenuManager : MonoBehaviour
	{
		public void SetAutoMove(bool Value)
		{
			SettingsManager.Instance.AutoMoveCards = Value;
		}

		public void SetSFXVolume(int Value)
		{
			SettingsManager.Instance.SFXVolume = Value;
		}

		public void SetMusicVolume(int Value)
		{
			SettingsManager.Instance.MusicVolume = Value;
		}

		public void SetHapticsStrength(int Value)
		{
			SettingsManager.Instance.HapticsStrength = Value;
		}
	}

}