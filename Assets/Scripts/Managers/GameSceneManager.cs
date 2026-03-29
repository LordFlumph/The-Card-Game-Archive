namespace CardGameArchive
{
	using System;
    using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        [SerializeField] Dictionary<GameTerms.GameName, int> gameScenes = new();

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
            SceneManager.LoadScene(gameScenes[gameName]);
        }
    }

}