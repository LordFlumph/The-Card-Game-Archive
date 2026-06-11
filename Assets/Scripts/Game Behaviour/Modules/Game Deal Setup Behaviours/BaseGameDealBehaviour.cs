namespace CardGameArchive.Behaviours
{
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class BaseGameDealBehaviour : ScriptableObject
	{
		[Tooltip("The delay between each card being dealt, in seconds.")]
		[SerializeField] protected float cardDealDelay = 0.05f;
		[SerializeField] protected float cardMoveTime = 0.15f;
		
		[SerializeField] protected GameTerms.DealDirection direction;

		[Tooltip("Should the moves be recorded for undo purposes?")]
		[SerializeField] protected bool recordMoves;

		public abstract Task DealCards();
		protected virtual async Task DealCard(Card card, ZoneParent destination, bool contingent = true)
		{
			GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, destination, timeToMove: cardMoveTime, canUndo: recordMoves, forceContingent: contingent, affectCardChain: false));
			await Awaitable.WaitForSecondsAsync(cardDealDelay);
		}
	}
}