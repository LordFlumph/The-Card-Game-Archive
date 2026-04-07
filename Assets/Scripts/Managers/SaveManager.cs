namespace CardGameArchive
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using UnityEngine;

	public class SaveManager
	{
		public static readonly string SAVE_PATH = Path.Join(Application.persistentDataPath, "game.sav");

		public class SaveDataBinder : ISerializationBinder
		{
			private readonly Assembly allowedAssembly;
			public SaveDataBinder()
			{
				allowedAssembly = typeof(SaveData).Assembly;
			}

			public void BindToName(Type serializedType, out string assemblyName, out string typeName)
			{
				assemblyName = null;
				typeName = serializedType.FullName;
			}

			public Type BindToType(string assemblyName, string typeName)
			{
				Type type = allowedAssembly.GetTypes().FirstOrDefault(o => o.FullName == typeName || o.Name == typeName);

				if (type == null)
					throw new JsonSerializationException($"Type `{typeName}` not found");

				if (!typeof(SaveData).IsAssignableFrom(type))
					throw new JsonSerializationException($"Type `{typeName}` is not a SaveData");

				return type;
			}
		}

		public static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All,
			Formatting = Formatting.Indented,
			SerializationBinder = new SaveDataBinder()
		};

		public static SaveFile LoadedFile { get; private set; } = new();

		public static void SaveGame()
		{
			if (LoadedFile.gameData.Count == 0)
				LoadGame();

			// Save Platform Data
			SaveFile saveFile = LoadedFile;

			if (BaseGameManager.Instance != null)
			{
				int gameIndex = (int)BaseGameManager.Instance.Name;

				if (saveFile.gameData[gameIndex].gameName != BaseGameManager.Instance.Name)
					throw new Exception("Error in reading save file");

				saveFile.gameData[gameIndex] = new();

				saveFile.gameData[gameIndex].gameName = BaseGameManager.Instance.Name;
				saveFile.gameData[gameIndex].saveData.Add(BaseGameManager.Instance.Save());
				saveFile.gameData[gameIndex].saveData.Add(GameBoard.Instance.Save());
			}

			var json = JsonConvert.SerializeObject(saveFile, JSON_SETTINGS);
			File.WriteAllText(SAVE_PATH, json);
		}

		public static SaveFile LoadGame()
		{
			SaveFile saveFile = new();
			saveFile.gameData.OrderBy(o => o.gameName);
			int totalGames = Enum.GetNames(typeof(GameTerms.GameName)).Length;
			if (saveFile.gameData.Count < totalGames)
			{
				for (int i = 0; i < totalGames; i++)
				{
					if ((int)saveFile.gameData[i].gameName != i)
					{
						GameSaveData newData = new();
						newData.gameName = (GameTerms.GameName)i;
					}
				}
			}
			LoadedFile = saveFile;
			return LoadedFile;
		}

		public static GameSaveData LoadGame(GameTerms.GameName gameName)
		{
			if (LoadedFile.gameData.Count == 0)
			{
				LoadGame();
			}

			if (LoadedFile.gameData.Count == 0)
			{
				return null;
			}

			if (LoadedFile.gameData.Any(o => o.gameName == gameName))
			{
				GameSaveData gameData = LoadedFile.gameData.First(o => o.gameName == gameName);
				if (gameData.saveData.Count != 0)
					return gameData;
			}

			return null;
		}
	}
	
	// Serves purely as an identifier
	[System.Serializable]
	public abstract class SaveData { }

	public class PlatformSaveData : SaveData
	{
		// Store things like win streaks, 
	}

	[System.Serializable]
	public class SaveFile
	{
		public PlatformSaveData platformData = new();
		public List<GameSaveData> gameData = new();
	}
	
	public class GameSaveData : SaveData
	{
		public GameTerms.GameName gameName;
		public List<SaveData> saveData = new();
		// Store game specific saves

		// Game Name (Klondike, Spider, etc.)
		// ZoneParent - Type (foundation, tableau, etc)
		// - Card
		//  - Data (rank, suit)
		//  - Flipped
		// - Card
		//  - Data (rank, suit)
		//  - Flipped
		// - Card
		//  - Data (rank, suit)
		//  - Flipped
		// ZoneParent - Type
		// etc.
		//
		// GameMove
		// - MoveType
		// - MoveCard

	}
}
