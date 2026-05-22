namespace CardGameArchive
{
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class IUndoBehaviour : ScriptableObject
	{
		public abstract Task UndoMove();
	}
}