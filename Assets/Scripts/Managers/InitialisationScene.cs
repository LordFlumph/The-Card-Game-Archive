using CardGameArchive;
using UnityEngine;

public class InitialisationScene : MonoBehaviour
{
	void Start()
	{
		GameTaskManager.Instance.QueueTask(() => { GameSceneManager.Instance.OpenMainMenu(); });
	}
}
