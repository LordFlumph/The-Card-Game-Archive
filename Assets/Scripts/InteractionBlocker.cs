using UnityEngine;

public class InteractionBlocker : MonoBehaviour
{
	public static InteractionBlocker Instance { get; private set; }

	new Collider2D collider;
	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		collider = GetComponent<Collider2D>();
		collider.enabled = false;
	}

	public void Activate() => collider.enabled = true;
	public void Deactivate() => collider.enabled = false;
}
