using System.Linq;
using UnityEngine;

[System.Serializable]
public class ToastService : MonoBehaviour
{
	public static ToastService Instance { get; private set; }

	[field: SerializeField] public Canvas toastCanvas { get; private set; }
	[field: SerializeField] public ToastPopup toastPopup { get; private set; }

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	public static void Show(string message, float duration = 2f)
	{
		if (Instance.toastCanvas == null)
		{
			Debug.LogError("No Canvas assigned for ToastService");
			return;
		}

		//ToastPopup toastPopup = Instantiate(Instance.toastPopupPrefab.gameObject, Instance.toastCanvas.transform).GetComponent<ToastPopup>();
		
		Instance.toastPopup.Create(message, duration);
	}
}
