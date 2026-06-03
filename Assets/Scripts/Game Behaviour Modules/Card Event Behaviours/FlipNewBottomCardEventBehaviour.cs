namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "FlipNewBottomCardEventBehaviour", menuName = "Game Behaviour/Card Event Behaviours/Flip New Bottom Card")]
	public class FlipNewBottomCardEventBehaviour : BaseCardEventBehaviour
	{
		[SerializeField] List<GameBoard.CardZone> zoneBlacklist;

		public override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from == null || zoneBlacklist.Contains(eventData.from.Zone))
				return;

			eventData.from.BottomCard.SetFlipped(true);
		}
	}
}