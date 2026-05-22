namespace CardGameArchive
{
	using UnityEngine;
	public abstract class IGameStateBehaviour : ScriptableObject
	{
		public abstract bool IsGameStuck();
	}
}
