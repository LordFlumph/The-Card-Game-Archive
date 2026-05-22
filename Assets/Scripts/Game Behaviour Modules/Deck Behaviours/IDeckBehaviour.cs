namespace CardGameArchive
{
	using UnityEngine;
	public abstract class IDeckBehaviour : ScriptableObject
	{
		public abstract void OnDeckTapped(Deck deck);
	}
}