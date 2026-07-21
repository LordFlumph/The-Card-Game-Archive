namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	public abstract class BaseRuntimeData : ScriptableObject, ISaveable
	{
		public virtual void Initialise() { }

		public abstract SaveData Save();
		public abstract void Load(SaveData saveData);

		public void LoadFailed(string reason)
		{
			StandardGameManager.Instance.LoadFailed(reason);
		}
	}
}