using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
	public static LoadingScreen Instance { get; private set; }
	[SerializeField] CanvasGroup canvasGroup;
	[SerializeField] float fadeTime = 0.1f;

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	public void Show()
	{
		canvasGroup.FadeIn(fadeTime);
	}
	public void Hide()
	{
		canvasGroup.FadeOut(fadeTime);
	}
}
