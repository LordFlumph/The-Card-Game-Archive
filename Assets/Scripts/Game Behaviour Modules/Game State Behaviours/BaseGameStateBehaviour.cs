namespace CardGameArchive.Behaviours
{
	using UnityEngine;
	public abstract class BaseGameStateBehaviour : ScriptableObject
	{
		public abstract bool IsGameStuck();
	}
}
