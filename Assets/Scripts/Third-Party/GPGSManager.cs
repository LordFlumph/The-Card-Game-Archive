using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSManager : MonoBehaviour
{
	public static GPGSManager Instance { get; private set; }

	public bool IsAuthenticated => PlayGamesPlatform.Instance.IsAuthenticated();

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			InitialiseGPGS();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void InitialiseGPGS()
	{

	}
}
