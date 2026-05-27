namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	public abstract class BasePostLoadBehaviour : ScriptableObject
	{
		public abstract bool PostLoad(SaveData saveData);
	}
}