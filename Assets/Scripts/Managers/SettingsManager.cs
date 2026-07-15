namespace CardGameArchive
{
    using UnityEngine;

    /// <summary>
    /// Handles all code related to managing Game settings
    /// </summary>
    public class SettingsManager : MonoBehaviour, ISaveable
    {
		public static SettingsManager Instance { get; private set; }

		public bool AutoMoveCards
		{
			get => autoMoveCards;
			set { autoMoveCards = value; SaveManager.Save(); }
		}
		public int SFXVolume
		{
			get => sfxVolume;
			set { sfxVolume = value; SaveManager.Save(); }
		}
		public int MusicVolume
		{
			get => musicVolume;
			set { musicVolume = value; SaveManager.Save(); }
		}
		public int HapticsStrength
		{
			get => hapticsStrength;
			set { hapticsStrength = value; SaveManager.Save(); }
		}

		private bool autoMoveCards = true;
		private int sfxVolume = 7;
		private int musicVolume = 7;
		private int hapticsStrength = 7;

		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		// Update is called once per frame
		void Update()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 120;
        }

		public class SettingsSaveData : SaveData
		{
			public bool autoMoveCards;
			public int sfxVolume;
			public int musicVolume;
			public int hapticsStrength;
		}

		public SaveData Save()
		{
			return new SettingsSaveData
			{
				autoMoveCards = AutoMoveCards,
				sfxVolume = SFXVolume,
				musicVolume = MusicVolume,
				hapticsStrength = HapticsStrength
			};
		}

		public void Load(SaveData saveData)
		{
			try
			{
				SettingsSaveData settingsSaveData = saveData as SettingsSaveData;
				autoMoveCards = settingsSaveData.autoMoveCards;
				sfxVolume = settingsSaveData.sfxVolume;
				musicVolume = settingsSaveData.musicVolume;
				hapticsStrength = settingsSaveData.hapticsStrength;
			}
			catch (System.Exception e)
			{
				LoadFailed(e.Message);
			}
		}

		public void LoadFailed(string reason)
		{
			throw new System.Exception(reason);
		}		
	}

}