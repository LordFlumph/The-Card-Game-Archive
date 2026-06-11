namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "FlipMirroredZoneNewBottomCardEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Card Event Behaviours/Flip Mirrored Zone New Bottom Card")]
	public class FlipMirroredZoneNewBottomCardEventBehaviour : BaseCardEventBehaviour
	{
		[SerializeField] GameBoard.CardZone firstZone, mirroredZone;
		[SerializeField] bool setInteractable = true;

		protected override void OnCardMoveFinish (GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from == null || eventData.to == null)
				return;

			if (eventData.to.Zone != firstZone)
				return;

			ZoneParent targetParent = GameBoard.Instance.GetZoneParent(mirroredZone, GameBoard.Instance.GetZoneIndex(eventData.to));
			if (targetParent.CardCount > 0)
			{
				targetParent.BottomCard.SetInteractable(setInteractable);
				GameTaskManager.Instance.AddTask(targetParent.BottomCard.SetFlipped(true));
			}
		}
	}
}