namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	public abstract class BasePostSetupBehaviour : ScriptableObject
	{
		public abstract void FinaliseBoard();
	}
}