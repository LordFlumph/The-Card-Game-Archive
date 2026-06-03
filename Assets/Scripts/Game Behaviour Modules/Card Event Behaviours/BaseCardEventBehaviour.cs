namespace CardGameArchive.Behaviours
{
	using UnityEngine;
	public abstract class BaseCardEventBehaviour : ScriptableObject
	{
		/// <summary>
		/// Activates when a Card begins its move. At this point, the card has left its 'from' zone but has not entered its 'to' zone
		/// </summary>
		public abstract void OnCardMoveStart(GameBoard.CardMoveEvent eventData);

		/// <summary>
		/// Activates when a Card finishes its move. At this point, the card has entered its 'to' zone
		/// </summary>
		public virtual void OnCardMoveFinish(GameBoard.CardMoveEvent eventData) { }
	}
}