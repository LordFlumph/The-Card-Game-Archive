using System.Collections.Generic;

namespace CardGameArchive
{
	public static class SaveManager
	{
		public static void SaveGame()
		{
			// Save Platform Data
			SaveFile saveFile = new();

			if (BaseGameManager.Instance != null)
			{
				// We are in a game, so lets save it
				BaseGameManager.Instance.Save();
			}
		}

		public static GameSaveData LoadGame(GameTerms.GameName gameName)
		{
			return null;
		}
	}
	public abstract class SaveData
	{

	}

	public class PlatformSaveData : SaveData
	{
		// Store things like win streaks, 
	}
	public class SaveFile
	{
		PlatformSaveData platformData = new();
		GameSaveData gameData = new();
	}
	public class GameSaveData : SaveData
	{
		GameTerms.GameName gameName;
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
