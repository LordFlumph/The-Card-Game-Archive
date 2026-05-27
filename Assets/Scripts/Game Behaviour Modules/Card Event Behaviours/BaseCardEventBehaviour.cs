namespace CardGameArchive.Behaviours
{
	using UnityEngine;
	public abstract class BaseCardEventBehaviour : ScriptableObject
	{
		public abstract void OnCardMoveStart(GameBoard.CardMoveEvent eventData);
		public abstract void OnCardMoveFinish(GameBoard.CardMoveEvent eventData);
	}
}