namespace CardGameArchive
{
	using UnityEngine;
	public abstract class ICardEventBehaviour : ScriptableObject
	{
		public abstract void OnCardMoveStart(GameBoard.CardMoveEvent eventData);
		public abstract void OnCardMoveFinish(GameBoard.CardMoveEvent eventData);
	}
}