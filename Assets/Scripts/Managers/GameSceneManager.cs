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

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		public void LoadGame(GameTerms.GameName gameName)
		{
			SceneManager.LoadScene(gameScenes.First(o => o.GameName == gameName).SceneIndex);
		}

		public void ReloadScene()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

}