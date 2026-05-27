namespace CardGameArchive.Behaviours
{
	using UnityEngine;
	public abstract class BaseDeckBehaviour : ScriptableObject
	{
		public abstract void OnDeckTapped(Deck deck);
	}
}