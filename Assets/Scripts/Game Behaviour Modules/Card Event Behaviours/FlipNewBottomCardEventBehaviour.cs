namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "FlipNewBottomCardEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Card Event Behaviours/Flip New Bottom Card")]
	public class FlipNewBottomCardEventBehaviour : BaseCardEventBehaviour
	{
		protected override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from == null || IsFromBlacklisted(eventData))
				return;

			if (eventData.from.CardCount > 0)
			{
				Card bottomCard = eventData.from.BottomCard;
				if (!bottomCard.Flipped)
				{
					if (eventData.canUndo)
						StandardGameManager.Instance.MoveTaken(new(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(bottomCard, true, true)));

					bottomCard.SetFlipped(true);
				}
			}
				
		}
	}
}