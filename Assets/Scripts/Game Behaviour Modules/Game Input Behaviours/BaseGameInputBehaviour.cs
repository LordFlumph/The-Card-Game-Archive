namespace CardGameArchive.Behaviours
{
	using UnityEngine;
	public abstract class BaseGameInputBehaviour : ScriptableObject
	{
		public abstract void OnCardTapped(Card card);
		public abstract void OnCardGrabbed(Card card);
		public abstract void OnCardDropped(Card card);
	}
}