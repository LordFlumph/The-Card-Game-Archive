namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	public abstract class BasePostLoadBehaviour : BaseBehaviour
	{
		public abstract bool PostLoad(SaveData saveData);
	}
}