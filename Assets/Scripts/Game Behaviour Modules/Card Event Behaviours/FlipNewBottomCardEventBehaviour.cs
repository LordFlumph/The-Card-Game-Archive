namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "FlipNewBottomCardEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Card Event Behaviours/Flip New Bottom Card")]
	public class FlipNewBottomCardEventBehaviour : BaseCardEventBehaviour
	{
		[SerializeField] List<GameBoard.CardZone> zoneBlacklist;

		public override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from == null || zoneBlacklist.Contains(eventData.from.Zone))
				return;

			if (eventData.from.CardCount > 0)
				eventData.from.BottomCard.SetFlipped(true);
		}
	}
}