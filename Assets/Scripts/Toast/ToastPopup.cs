using TMPro;
using UnityEngine;

public class ToastPopup : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI textField;
	[SerializeField] CanvasGroup canvasGroup;

	float lifetime;
	bool alive = false;

	public void Create(string message, float duration)
	{
		textField.text = message;
		lifetime = duration;
		alive = true;
		canvasGroup.FadeIn(0.2f);
	}

	void Update()
	{
		if (!alive)
			return;

		lifetime -= Time.deltaTime;
		if (lifetime <= 0)
		{
			DestroyPopup();
		}
	}

	public void DestroyPopup()
	{
		alive = false;
		canvasGroup.FadeOut(0.2f);
	}
}
