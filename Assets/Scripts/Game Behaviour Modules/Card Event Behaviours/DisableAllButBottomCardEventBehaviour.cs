namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "DisableAllButBottomCardEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Card Event Behaviours/Disable All But Bottom Card")]
	public class DisableAllButBottomCardEventBehaviour : BaseCardEventBehaviour
	{
		[SerializeField] List<GameBoard.CardZone> zoneBlacklist;

		public override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from == null || zoneBlacklist.Contains(eventData.from.Zone))
				return;

			if (eventData.from.CardCount > 0)
				eventData.from.BottomCard.SetInteractable(true);
		}

		public override void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.to == null || zoneBlacklist.Contains(eventData.to.Zone))
				return;

			foreach (var card in eventData.to.Cards)
			{
				card.Data.SetInteractable(false);
			}

			if (eventData.to.CardCount > 0)
				eventData.to.BottomCard.SetInteractable(true);

		}
	}

}