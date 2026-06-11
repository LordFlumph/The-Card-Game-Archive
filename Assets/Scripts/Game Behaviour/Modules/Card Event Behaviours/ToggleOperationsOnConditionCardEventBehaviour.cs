namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "ToggleOperationsOnConditionCardEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Card Event Behaviours/Toggle Operations On Condition")]
	public class ToggleOperationsOnConditionCardEventBehaviour : BaseCardEventBehaviour
	{
		[SerializeField] BaseCondition condition;
		[SerializeField] GameBoard.CardZone targetZone;

		protected override async void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from == null || IsFromBlacklisted(eventData))
				return;			

			foreach (ZoneParent parent in GameBoard.Instance.GetZoneParents(targetZone))
			{
				parent.UseOperations = condition.ConditionMet(parent);
			}
		}
	}
}