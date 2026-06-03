namespace CardGameArchive.Behaviours
{
	using CardGameArchive.TMP;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "FlipCardUpEventBehaviour", menuName = "Game Behaviour/Card Event Behaviours/Flip Card Up On Move")]
	public class FlipCardUpEventBehaviour : BaseCardEventBehaviour
	{
		[SerializeField] bool waitForGameStart = true;
		[SerializeField] List<GameBoard.CardZone> zoneBlacklist;
		[SerializeField] bool blacklistFrom, blacklistTo;
		public override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (waitForGameStart && !BaseGameManager.Instance.GamePlaying)
				return;

			if (eventData.card == null)
				return;

			bool blacklisted = (blacklistFrom && zoneBlacklist.Contains(eventData.from.Zone)) || (blacklistTo && zoneBlacklist.Contains(eventData.to.Zone));
			if (blacklisted)
				return;

			eventData.card.SetFlipped(true);			

			if (eventData.canUndo)
				StandardGameManager.Instance.MoveTaken(new(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(eventData.from.BottomCard, true, true)));
		}
	}
}