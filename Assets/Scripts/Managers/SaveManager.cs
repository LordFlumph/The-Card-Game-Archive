namespace CardGameArchive
{
	using CardGameArchive.MainMenu;
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

		public const string SAVE_VERSION = "1.2";

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
				Load();

			SaveFile saveFile = ActiveFile ?? new();

			saveFile.platformData.saveVersion = SAVE_VERSION;
			saveFile.platformData.gameVersion = Application.version;
			saveFile.platformData.settingsData = SettingsManager.Instance.Save();
			saveFile.platformData.gameData = GameDataManager.Instance.Save();
			if (StandardGameManager.Instance != null)
				saveFile.platformData.lastPlayed = StandardGameManager.Instance.Variant;


			// Save Game Data
			if (StandardGameManager.Instance != null && StandardGameManager.Instance.CanSave)
			{
				GameSaveData gameData = new();
				gameData.gameVariant = StandardGameManager.Instance.Variant;

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

				int gameIndex = saveFile.gameData.FindIndex(o => o.gameVariant == StandardGameManager.Instance.Variant);
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

		public static void ClearGameSave(GameTerms.GameVariant gameVariant)
		{
			if (ActiveFile == null)
				Load();

			if (ActiveFile == null)
				return;

			int targetIndex = ActiveFile.gameData.FindIndex(o => o.gameVariant == gameVariant);
			if (targetIndex == -1)
				return;

			ActiveFile.gameData[targetIndex] = new();
			WriteActiveToFile();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void Load()
		{
			Debug.Log("Loading save file");
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

				LoadPlatformData();
			}
			catch (Exception e)
			{
				Debug.LogError("Unknown error loading save file: " + e.Message);
			}
		}

		static void LoadPlatformData()
		{
			if (ActiveFile == null)
				return;

			GameDataManager.Instance.Load(ActiveFile.platformData.gameData);
			SettingsManager.Instance.Load(ActiveFile.platformData.settingsData);
		}

		public static GameSaveData LoadGame(GameTerms.GameVariant gameVariant)
		{
			if (ActiveFile == null)
				Load();

			if (ActiveFile == null)
				return null;

			int gameIndex = ActiveFile.gameData.FindIndex(o => o.gameVariant == gameVariant); // Assuming gameVariant has a property to get the gameName

			if (gameIndex != -1)
			{
				if (ActiveFile.gameData[gameIndex].gameManagerData != null && ActiveFile.gameData[gameIndex].gameBoardData != null)
					return ActiveFile.gameData[gameIndex];
			}

			Debug.Log("Unable to locate save data for " + gameVariant);
			return null;
		}
	}

	// Serves purely as an identifier
	[Serializable]
	public abstract class SaveData { }

	public class PlatformSaveData : SaveData
	{
		public string gameVersion;
		public string saveVersion;
		public SaveData gameData;
		public SaveData settingsData;
		public GameTerms.GameVariant lastPlayed;
	}

	[Serializable]
	public class SaveFile
	{
		public PlatformSaveData platformData = new();
		public List<GameSaveData> gameData = new();
	}

	public class GameSaveData : SaveData
	{
		public GameTerms.GameVariant gameVariant;
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
