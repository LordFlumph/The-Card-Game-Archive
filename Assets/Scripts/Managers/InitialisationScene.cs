using CardGameArchive;
using UnityEngine;

public class InitialisationScene : MonoBehaviour
{
	void Start()
	{
		GameSceneManager.Instance.OpenMainMenu();
	}
}
