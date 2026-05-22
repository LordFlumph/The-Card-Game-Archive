namespace CardGameArchive
{
	using UnityEngine;
	public abstract class IGameInputBehaviour : ScriptableObject
	{
		public abstract void OnCardTapped(Card card);
		public abstract void OnCardGrabbed(Card card);
		public abstract void OnCardDropped(Card card);
	}
}