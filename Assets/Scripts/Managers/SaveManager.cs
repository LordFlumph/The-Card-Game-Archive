namespace CardGameArchive
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Newtonsoft.Json.Serialization;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using UnityEngine;

	public class SaveManager
	{
		public static readonly string SAVE_PATH = Path.Join(Application.persistentDataPath, "game.sav");
		public static readonly string TEMP_PATH = Path.Join(Application.persistentDataPath, "game.tmp");
		public static readonly string BACKUP_PATH = Path.Join(Application.persistentDataPath, "game.bak");

		public const string SAVE_VERSION = "1.0";

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

		public static SaveFile ActiveFile { get; private set; } = null;

		public static void Save()
		{
			Debug.Log("Saving");
			if (ActiveFile == null)
				LoadGame();

			SaveFile saveFile = ActiveFile ?? new();

			// TODO: Save Platform Data
			saveFile.platformData.saveVersion = SAVE_VERSION;
			saveFile.platformData.gameVersion = Application.version;
			saveFile.platformData.settingsData = SettingsManager.Instance.Save();


			// Save Game Data
			if (StandardGameManager.Instance != null && StandardGameManager.Instance.CanSave)
			{
				GameSaveData gameData = new();
				gameData.gameName = StandardGameManager.Instance.Name;

				SaveData saveData = StandardGameManager.Instance.Save();
				if (saveData != null)
					gameData.gameManagerData = saveData;
				else
					Debug.LogWarning("GameManager returned null on save");

				saveData = GameBoard.Instance.Save();
				if (saveData != null)
					gameData.gameBoardData = saveData;
				else
					Debug.LogWarning("GameBoard returned null on save");

				int gameIndex = saveFile.gameData.FindIndex(o => o.gameName == StandardGameManager.Instance.Name);
				if (gameIndex == -1)
					saveFile.gameData.Add(gameData);
				else
					saveFile.gameData[gameIndex] = gameData;
			}

			ActiveFile = saveFile;
			WriteActiveToFile();
		}

		static void WriteActiveToFile()
		{
			var json = JsonConvert.SerializeObject(ActiveFile, JSON_SETTINGS);
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

		public static void ClearGameSave(GameTerms.GameName gameName)
		{
			if (ActiveFile == null)
				LoadGame();

			if (ActiveFile == null)
				return;

			int targetIndex = ActiveFile.gameData.FindIndex(o => o.gameName == gameName);
			if (targetIndex == -1)
				return;

			ActiveFile.gameData[targetIndex] = new();
			WriteActiveToFile();
		}

		public static void LoadGame()
		{
			SaveFile saveFile;
			ActiveFile = null;

			if (!File.Exists(SAVE_PATH))
				return;

			try
			{
				string json = File.ReadAllText(SAVE_PATH);

				// Check version compatibility
				JObject root = JObject.Parse(json);
				string version = root["platformData"]?["saveVersion"]?.Value<string>() ?? "";
				if (version != SAVE_VERSION)
				{
					Debug.Log("Save file version mismatch");
					return;
				}

				saveFile = JsonConvert.DeserializeObject<SaveFile>(json, JSON_SETTINGS);
				ActiveFile = saveFile;
			}
			catch (Exception e)
			{
				Debug.LogError("Unknown error loading save file: " + e.Message);
			}
		}

		public static GameSaveData LoadGame(GameTerms.GameName gameName)
		{
			if (ActiveFile == null)
				LoadGame();

			if (ActiveFile == null)
				return null;

			int gameIndex = ActiveFile.gameData.FindIndex(o => o.gameName == gameName);

			if (gameIndex != -1)
			{
				if (ActiveFile.gameData[gameIndex].gameManagerData != null && ActiveFile.gameData[gameIndex].gameBoardData != null)
					return ActiveFile.gameData[gameIndex];
			}

			Debug.Log("Unable to locate save data for " + gameName);
			return null;
		}
	}

	// Serves purely as an identifier
	[Serializable]
	public abstract class SaveData { }

	public class PlatformSaveData : SaveData
	{
		// Store things like win streaks,
		[Serializable]
		public class GameStats
		{
			public GameTerms.GameName gameName;
			public bool unlocked;
			public int wins;
			public int losses;
			public int winstreak;
		}

		public string gameVersion;
		public string saveVersion;
		public List<GameStats> gameStats = new();
		public SaveData settingsData;
	}

	[Serializable]
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
