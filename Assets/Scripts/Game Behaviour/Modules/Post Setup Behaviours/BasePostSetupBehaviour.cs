namespace CardGameArchive.Behaviours
{
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class BasePostSetupBehaviour : ScriptableObject
	{
		public abstract Task FinaliseBoard();
	}
}