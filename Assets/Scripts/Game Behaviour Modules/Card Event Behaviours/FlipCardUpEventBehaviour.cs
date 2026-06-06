namespace CardGameArchive.Behaviours
{
	
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "FlipCardUpEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Card Event Behaviours/Flip Card Up On Move")]
	public class FlipCardUpEventBehaviour : BaseCardEventBehaviour
	{
		[SerializeField] bool waitForGameStart = true;
		[SerializeField] List<GameBoard.CardZone> zoneBlacklist;
		[SerializeField] bool blacklistFrom, blacklistTo;
		public override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (waitForGameStart && !StandardGameManager.Instance.GamePlaying)
				return;

			if (eventData.card == null)
				return;

			bool blacklisted = (blacklistFrom && zoneBlacklist.Contains(eventData.from.Zone)) || (blacklistTo && zoneBlacklist.Contains(eventData.to.Zone));
			if (blacklisted)
				return;

			if (!eventData.card.Flipped)
			{
				eventData.card.SetFlipped(true);

				if (eventData.canUndo)
					StandardGameManager.Instance.MoveTaken(new(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(eventData.card, true, true)));
			}			
		}
	}
}