namespace CardGameArchive
{
    using UnityEngine;

    /// <summary>
    /// Handles all code related to managing Game settings
    /// </summary>
    public class SettingsManager : MonoBehaviour, ISaveable
    {
		public static SettingsManager Instance { get; private set; }

        public bool AutoMoveCards { get; private set; } = true;

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
		}

		public SaveData Save()
		{
			SettingsSaveData saveData = new SettingsSaveData();
			saveData.autoMoveCards = AutoMoveCards;
			return saveData;
		}

		public void Load(SaveData saveData)
		{
			try
			{
				SettingsSaveData settingsSaveData = saveData as SettingsSaveData;
				AutoMoveCards = settingsSaveData.autoMoveCards;
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