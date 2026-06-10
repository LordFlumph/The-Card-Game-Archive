namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "DisableAllButBottomCardEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Card Event Behaviours/Disable All But Bottom Card")]
	public class DisableAllButBottomCardEventBehaviour : BaseCardEventBehaviour
	{
		[Tooltip("If true, the card colour will change when disabled")]
		[SerializeField] bool changeColour = true;
		protected override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from == null || IsFromBlacklisted(eventData))
				return;

			foreach (var card in eventData.from.Cards)
			{
				card.Data.SetInteractable(false, changeColour);
			}

			if (eventData.from.CardCount > 0)
				eventData.from.BottomCard.SetInteractable(true);
		}

		protected override void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.to == null || IsToBlacklisted(eventData))
				return;

			foreach (var card in eventData.to.Cards)
			{
				card.Data.SetInteractable(false, changeColour);
			}

			if (eventData.to.CardCount > 0)
				eventData.to.BottomCard.SetInteractable(true);
		}
	}

}