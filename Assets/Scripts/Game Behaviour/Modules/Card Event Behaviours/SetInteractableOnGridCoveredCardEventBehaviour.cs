namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "SetInteractableOnGridCoveredCardEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Card Event Behaviours/Set Interactable On Grid Covered")]
	public class SetInteractableOnGridCoveredCardEventBehaviour : BaseCardEventBehaviour
	{
		protected override void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			ZoneGridRuntimeData gridData = StandardGameManager.Instance.GetRuntimeData<ZoneGridRuntimeData>();

			if (gridData == null)
			{
				throw new System.Exception("ZoneGridRuntimeData not found in StandardGameManager.");
			}

			if (eventData.from.Zone != gridData.CardZone && eventData.to.Zone != gridData.CardZone)
				return; // Nothing changed, no need to run this code

			foreach (ZoneParent zone in GameBoard.Instance.GetZoneParents(gridData.CardZone))
			{
				if (gridData.IsZoneCovered(zone))
				{
					foreach (CardObject card in zone.Cards)
					{
						card.Data.SetInteractable(false);
					}
				}
				else
				{
					foreach (CardObject card in zone.Cards)
					{
						card.Data.SetInteractable(true);
					}
				}
			}
		}
	}

}