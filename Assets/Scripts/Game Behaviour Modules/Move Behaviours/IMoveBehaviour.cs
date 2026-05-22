namespace CardGameArchive
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;
	public abstract class IMoveBehaviour : ScriptableObject
	{
		public abstract List<ZoneParent> GetPossibleMoves(Card card);

		/// <summary>
		/// Automatically move any cards that can be moved
		/// </summary>
		public abstract void AutoMoveAny();

		/// <summary>
		/// Automatically move this card somewhere it will 
		/// </summary>
		/// <param name="card"></param>
		/// <param name="playerDriven">Whether this move was triggered directly by the player</param>
		/// <returns></returns>
		public abstract Task AutoMove(Card card, bool playerDriven = true);
	}
}