namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;
	public abstract class BaseCardEventBehaviour : ScriptableObject
	{
		[SerializeField] protected List<GameBoard.CardZone> zoneBlacklist;
		[SerializeField] protected bool blacklistFrom, blacklistTo;

		/// <summary>
		/// Activates when a Card begins its move. At this point, the card has left its 'from' zone but has not entered its 'to' zone
		/// </summary>
		public virtual void OnCardMoveStart(GameBoard.CardMoveEvent eventData) { }

		/// <summary>
		/// Activates when a Card finishes its move. At this point, the card has entered its 'to' zone
		/// </summary>
		public virtual void OnCardMoveFinish(GameBoard.CardMoveEvent eventData) { }

		protected virtual bool IsFromBlacklisted(GameBoard.CardMoveEvent eventData)
		{
			return (blacklistFrom && zoneBlacklist.Contains(eventData.from.Zone));
		}
		protected virtual bool IsToBlacklisted(GameBoard.CardMoveEvent eventData)
		{
			return (blacklistTo && zoneBlacklist.Contains(eventData.to.Zone));
		}
		protected virtual bool IsBlacklisted(GameBoard.CardMoveEvent eventData) => IsFromBlacklisted(eventData) || IsToBlacklisted(eventData);
	}
}