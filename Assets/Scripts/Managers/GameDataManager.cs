namespace CardGameArchive
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class GameDataManager : MonoBehaviour, ISaveable
	{
		public static GameDataManager Instance { get; private set; }

		public event Action OnFavouritesChanged;
		public event Action<GameTerms.GameVariant, bool> OnFavouriteSet;

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
			[field: SerializeField] public GameInfo LinkedInfo { get; private set; }
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

		public GameInfo GetGameInfo(GameTerms.GameVariant variant)
		{
			if (variant == GameTerms.GameVariant.NONE)
				return null;

			// We aren't using FirstOrDefault here because if a variant isn't found, then that is a major error
			return Data.First(o => o.Variant == variant).LinkedInfo;
		}

		public void SetFavourite(GameTerms.GameVariant variant, bool favourite)
		{
			GameStats stats = Data.FirstOrDefault(o => o.Variant == variant);
			if (stats != null)
			{
				stats.Favourite = favourite;
				OnFavouritesChanged?.Invoke();
				OnFavouriteSet?.Invoke(variant, favourite);
			}
		}
		public bool IsFavourite(GameTerms.GameVariant variant)
		{
			GameStats stats = Data.FirstOrDefault(o => o.Variant == variant);
			return stats?.Favourite ?? false;
		}
		public List<GameTerms.GameVariant> GetFavourites() => Data.Where(o => o.Favourite).Select(o => o.Variant).ToList();

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
