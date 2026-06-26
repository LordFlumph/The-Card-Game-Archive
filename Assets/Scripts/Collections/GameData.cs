namespace CardGameArchive
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class GameData : MonoBehaviour, ISaveable
	{
		[System.Serializable]
		public class GameStats : ISaveable
		{
			public GameTerms.GameVariant Variant;
			public bool Favourite;

			public SaveData Save()
			{
				throw new System.NotImplementedException();
			}
			public void Load(SaveData saveData)
			{
				throw new System.NotImplementedException();
			}

			public void LoadFailed(string reason)
			{
				throw new System.NotImplementedException();
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

		public SaveData Save()
		{
			throw new System.NotImplementedException();
		}
		public void Load(SaveData saveData)
		{
			throw new System.NotImplementedException();
		}

		public void LoadFailed(string reason)
		{
			throw new System.NotImplementedException();
		}		
	}
}
