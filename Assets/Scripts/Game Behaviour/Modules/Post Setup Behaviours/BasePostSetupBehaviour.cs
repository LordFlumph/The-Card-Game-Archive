namespace CardGameArchive.Behaviours
{
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class BasePostSetupBehaviour : BaseBehaviour
	{
		public abstract Task FinaliseBoard();
	}
}