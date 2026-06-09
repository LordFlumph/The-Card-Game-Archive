namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class BaseUndoBehaviour : ScriptableObject
	{
		[SerializeField] protected float contingentUndoDelay = 0.05f;
		public virtual async Task UndoMove(Stack<GameMove> gameMoves)
		{
			if (gameMoves.Count == 0)
			{
				Debug.LogWarning("No moves to undo.");
				return;
			}

			GameMove lastMove = gameMoves.Pop();
			switch (lastMove.type)
			{
				case GameMove.MoveType.CardFlipped:
					UndoCardFlipped(lastMove.Data as GameMove.CardFlippedData);
					break;
				case GameMove.MoveType.CardMoved:
					UndoCardMoved(lastMove.Data as GameMove.CardMovedData);
					break;
			}

			UIManager.Instance.HideGameStuck();

			if (lastMove.Contingent)
			{
				if (lastMove.Data.cardData != gameMoves.Peek().Data.cardData)
					await Awaitable.WaitForSecondsAsync(contingentUndoDelay);

				await UndoMove(gameMoves);
			}
		}

		protected abstract void UndoCardFlipped(GameMove.CardFlippedData flippedData);
		protected abstract void UndoCardMoved(GameMove.CardMovedData movedData);
	}
}