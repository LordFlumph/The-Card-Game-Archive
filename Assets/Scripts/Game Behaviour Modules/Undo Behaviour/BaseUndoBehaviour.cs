namespace CardGameArchive.Behaviours
{
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class BaseUndoBehaviour : ScriptableObject
	{
		public abstract Task UndoMove();
	}
}