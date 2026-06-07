namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "FlipCardUpEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Card Event Behaviours/Flip Card Up On Move")]
	public class FlipCardUpEventBehaviour : BaseCardEventBehaviour
	{
		[SerializeField] bool waitForGameStart = true;
		public override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (waitForGameStart && !StandardGameManager.Instance.GamePlaying)
				return;

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