namespace CardGameArchive
{
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class IGameSetupBehaviour : ScriptableObject
	{
		public abstract Task DealCards();
		public abstract void FinaliseBoard();
	}
}