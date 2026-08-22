namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;
	public abstract class BaseCardEventBehaviour : ScriptableObject
	{
		[SerializeField] List<BaseBehaviourBlocker> blockingConditions;

		[SerializeField] protected List<GameBoard.CardZone> zoneBlacklist;
		[SerializeField] protected bool blacklistFrom, blacklistTo;

		[SerializeField] protected bool waitForGameStart = true;

		/// <summary>
		/// Activates when a Card begins its move. At this point, the card has left its 'from' zone but has not entered its 'to' zone
		/// </summary>

		public void CardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if ((waitForGameStart && !StandardGameManager.Instance.GamePlaying) || StandardGameManager.Instance.GameRestarting)
				return;

			foreach (BaseBehaviourBlocker blocker in blockingConditions)
			{
				if (blocker.BlockBehaviour())
					return;
			}
			OnCardMoveStart(eventData);
		}
		protected virtual void OnCardMoveStart(GameBoard.CardMoveEvent eventData) { }

		/// <summary>
		/// Activates when a Card finishes its move. At this point, the card has entered its 'to' zone
		/// </summary>
		public void CardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			if ((waitForGameStart && !StandardGameManager.Instance.GamePlaying) || StandardGameManager.Instance.GameRestarting)
				return;

			foreach (BaseBehaviourBlocker blocker in blockingConditions)
			{
				if (blocker.BlockBehaviour())
					return;
			}
			OnCardMoveFinish(eventData);
		}
		protected virtual void OnCardMoveFinish(GameBoard.CardMoveEvent eventData) { }

		protected virtual bool IsFromBlacklisted(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from == null)
				return true;

			return blacklistFrom && zoneBlacklist.Contains(eventData.from.Zone);
		}
		protected virtual bool IsToBlacklisted(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.to == null)
				return true;

			return blacklistTo && zoneBlacklist.Contains(eventData.to.Zone);
		}
		protected virtual bool IsBlacklisted(GameBoard.CardMoveEvent eventData) => IsFromBlacklisted(eventData) || IsToBlacklisted(eventData);
	}
}