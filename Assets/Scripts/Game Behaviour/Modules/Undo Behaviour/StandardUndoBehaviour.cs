namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "StandardUndoBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Undo Behaviours/Standard")]
	public class StandardUndoBehaviour : BaseUndoBehaviour
	{
		protected override void UndoCardFlipped(GameMove.CardFlippedData flippedData)
		{
			GameTaskManager.Instance.AddTask(flippedData.cardData.SetFlipped(!flippedData.flipped));
			flippedData.cardData.SetInteractable(!flippedData.flipped);	
		}

		protected override void UndoCardMoved(GameMove.CardMovedData movedData)
		{
			GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(movedData.cardData, movedData.from, canUndo: false));

			if (movedData.from.Zone == GameBoard.CardZone.Stock)
			{
				Deck deck = movedData.from.GetComponent<DeckObject>().Data;
				deck.AddCard(movedData.cardData);
			}
			if (movedData.to.Zone == GameBoard.CardZone.Stock)
			{
				Deck deck = movedData.to.GetComponent<DeckObject>().Data;
				deck.SyncCards();
			}
		}
	}
}