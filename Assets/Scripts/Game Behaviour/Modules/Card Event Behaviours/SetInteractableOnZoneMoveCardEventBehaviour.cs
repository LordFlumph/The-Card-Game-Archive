namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "SetInteractableOnZoneMoveCardEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Card Event Behaviours/Set Interactable On Zone Move")]
	public class SetInteractableOnZoneMoveCardEventBehaviour : BaseCardEventBehaviour
	{
		[SerializeField] bool invertInteractableIfBlacklisted = false;

		[SerializeField] bool interactable = false;
		[SerializeField] bool changeColour = false;

		protected override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (!IsFromBlacklisted(eventData))
			{
				eventData.card.SetInteractable(interactable, changeColour);
			}
		}

		protected override void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			if (!IsToBlacklisted(eventData))
			{
				eventData.card.SetInteractable(interactable, changeColour);
			}
		}
	}

}