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
		public static readonly string TEMP_PATH = Path.Join(Application.persistentDataPath, "game.tmp");
		public static readonly string BACKUP_PATH = Path.Join(Application.persistentDataPath, "game.bak");

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
			TypeNameHandling = TypeNameHandling.Auto,
			Formatting = Formatting.Indented,
			SerializationBinder = new SaveDataBinder()
		};

		public static SaveFile LoadedFile { get; private set; } = null;

		public static void SaveGame()
		{
			Debug.Log("Saving");
			if (LoadedFile == null)
				LoadGame();

			// Save Platform Data
			SaveFile saveFile = LoadedFile ?? new();

			if (BaseGameManager.Instance != null)
			{
				GameSaveData gameData = new();
				gameData.gameName = BaseGameManager.Instance.Name;

				SaveData saveData = BaseGameManager.Instance.Save();
				if (saveData != null)
					gameData.gameManagerData = saveData;
				else
					Debug.LogWarning("GameManager returned null on save, there is likely an error in the save function");

				saveData = GameBoard.Instance.Save();
				if (saveData != null)
					gameData.gameBoardData = saveData;
				else
					Debug.LogWarning("GameBoard returned null on save, there is likely an error in the save function");

				int gameIndex = saveFile.gameData.FindIndex(o => o.gameName == BaseGameManager.Instance.Name);
				if (gameIndex == -1)
					saveFile.gameData.Add(gameData);
				else
					saveFile.gameData[gameIndex] = gameData;
			}

			var json = JsonConvert.SerializeObject(saveFile, JSON_SETTINGS);
			File.WriteAllText(TEMP_PATH, json);

			if (File.Exists(SAVE_PATH))
			{
				File.Replace(TEMP_PATH, SAVE_PATH, BACKUP_PATH, true);
			}
			else
			{
				File.Move(TEMP_PATH, SAVE_PATH);
			}
		}

		public static SaveFile LoadGame()
		{
			SaveFile saveFile = new();
			if (File.Exists(SAVE_PATH))
			{
				string json = File.ReadAllText(SAVE_PATH);
				saveFile = JsonConvert.DeserializeObject<SaveFile>(json, JSON_SETTINGS);

				LoadedFile = saveFile;
				return saveFile;
			}

			return null;
		}

		public static GameSaveData LoadGame(GameTerms.GameName gameName)
		{
			if (LoadedFile == null || LoadedFile.gameData.Count == 0)
			{
				LoadGame();
			}

			if (LoadedFile == null || LoadedFile.gameData.Count == 0)
			{
				return null;
			}

			if (LoadedFile.gameData.Any(o => o.gameName == gameName))
			{
				GameSaveData gameData = LoadedFile.gameData.First(o => o.gameName == gameName);
				if (gameData.gameManagerData != null && gameData.gameBoardData != null)
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
		public SaveData gameManagerData;
		public SaveData gameBoardData;
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
