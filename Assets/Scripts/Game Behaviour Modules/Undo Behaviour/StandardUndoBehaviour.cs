namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	public class StandardUndoBehaviour : BaseUndoBehaviour
	{
		public override void UndoCardFlipped(GameMove.CardFlippedData flippedData)
		{
			GameTaskManager.Instance.AddTask(flippedData.cardData.SetFlipped(!flippedData.flipped));
			flippedData.cardData.SetInteractable(!flippedData.flipped);	
		}

		public override void UndoCardMoved(GameMove.CardMovedData movedData)
		{
			GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(movedData.cardData, movedData.from, canUndo: false));
		}
	}
}