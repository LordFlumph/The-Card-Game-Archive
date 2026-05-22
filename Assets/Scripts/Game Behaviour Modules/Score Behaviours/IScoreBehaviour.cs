namespace CardGameArchive
{
	using UnityEngine;
	public abstract class IScoreBehaviour : ScriptableObject
	{
		public abstract int GetScore();
	}
}
