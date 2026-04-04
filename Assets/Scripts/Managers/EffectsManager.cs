namespace CardGameArchive
{
    using UnityEngine;

	/// <summary>
	/// Manages all code related to visual effects
	/// </summary>
    public class EffectsManager : MonoBehaviour
    {
		public static EffectsManager Instance { get; private set; }
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

		// Update is called once per frame
		void Update()
        {

        }
    }

}