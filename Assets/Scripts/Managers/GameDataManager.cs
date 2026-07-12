namespace CardGameArchive
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class GameDataManager : MonoBehaviour, ISaveable
	{
		public static GameDataManager Instance { get; private set; }
		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		[System.Serializable]
		public class GameStats : ISaveable
		{
			public GameTerms.GameVariant Variant;
			public bool Favourite;

			public class GameStatsSaveData : SaveData
			{
				public GameStatsSaveData(GameTerms.GameVariant variant, bool favourite)
				{
					Variant = variant;
					Favourite = favourite;
				}
				public GameTerms.GameVariant Variant;
				public bool Favourite;
			}
			public SaveData Save()
			{
				return new GameStatsSaveData(Variant, Favourite);
			}
			public void Load(SaveData saveData)
			{
				GameStatsSaveData gameData = saveData as GameStatsSaveData;
				if (gameData != null)
				{
					Favourite = gameData.Favourite;
				}
			}

			public void LoadFailed(string reason)
			{
				throw new System.Exception(reason);
			}
		}

		[SerializeField] List<GameStats> Data = new();

		public void SetFavourite(GameTerms.GameVariant variant, bool favourite)
		{
			GameStats stats = Data.FirstOrDefault(o => o.Variant == variant);
			if (stats != null)
				stats.Favourite = favourite;
		}
		public bool IsFavourite(GameTerms.GameVariant variant)
		{
			GameStats stats = Data.FirstOrDefault(o => o.Variant == variant);
			return stats?.Favourite ?? false;
		}


		public class GameDataSaveData : SaveData
		{
			public GameDataSaveData(List<GameStats.GameStatsSaveData> data)
			{
				Data = data;
			}
			public List<GameStats.GameStatsSaveData> Data;
		}
		public SaveData Save()
		{
			List<GameStats.GameStatsSaveData> gameStatsSaveData = new();
			foreach (var stats in Data)
			{
				gameStatsSaveData.Add(stats.Save() as GameStats.GameStatsSaveData);
			}
			return new GameDataSaveData(gameStatsSaveData);
		}
		public void Load(SaveData saveData)
		{
			GameDataSaveData gameData = saveData as GameDataSaveData;
			if (gameData != null)
			{
				for (int i = 0; i < Data.Count; i++)
				{
					Data[i].Load(gameData.Data[i]);
				}
			}
		}

		public void LoadFailed(string reason)
		{
			throw new System.Exception(reason);
		}		
	}
}
