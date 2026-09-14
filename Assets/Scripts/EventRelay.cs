using UnityEngine;
using UnityEngine.Events;

public class EventRelay : MonoBehaviour
{
	public UnityEvent onTrigger = new();
	public void Trigger()
	{
		onTrigger.Invoke();
	}
}
