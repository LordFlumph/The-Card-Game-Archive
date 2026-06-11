namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "FlipCardUpEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Card Event Behaviours/Flip Card Up On Move")]
	public class FlipCardUpEventBehaviour : BaseCardEventBehaviour
	{
		protected override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.card == null || IsBlacklisted(eventData))
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