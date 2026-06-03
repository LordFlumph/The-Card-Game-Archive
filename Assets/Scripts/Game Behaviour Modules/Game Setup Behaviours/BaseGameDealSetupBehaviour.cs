namespace CardGameArchive.Behaviours
{
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class BaseGameDealSetupBehaviour : ScriptableObject
	{
		[Tooltip("The delay between each card being dealt, in seconds.")]
		[SerializeField] protected float cardDealDelay = 0.05f;
		[SerializeField] protected float cardMoveTime = 0.15f;
		public abstract Task DealCards();
		protected virtual async Task DealCard(Card card, ZoneParent destination)
		{
			GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, destination, timeToMove: cardMoveTime, canUndo: false, affectCardChain: false));
			await Awaitable.WaitForSecondsAsync(cardDealDelay);
		}
	}
}