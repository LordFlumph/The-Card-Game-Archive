namespace CardGameArchive
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	/// <summary>
	/// Handles all code related to managing game scenes
	/// </summary>
	public class GameSceneManager : MonoBehaviour
	{
		public static GameSceneManager Instance { get; private set; }

		[Serializable]
		struct GameSceneData
		{
			public GameTerms.GameName GameName;
			public int SceneIndex;
		}


		[SerializeField] List<GameSceneData> gameScenes = new();

		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		public void OpenGame(GameTerms.GameName gameName)
		{
			Card.ResetIDCounter();
			SceneManager.LoadScene(gameScenes.First(o => o.GameName == gameName).SceneIndex);
		}

		public void ReloadScene()
		{
			if (BaseGameManager.Instance != null)
				OpenGame(BaseGameManager.Instance.Name);
			else
				Debug.LogError("Cannot reload a scene that is missing a GameManager");
		}
	}

}