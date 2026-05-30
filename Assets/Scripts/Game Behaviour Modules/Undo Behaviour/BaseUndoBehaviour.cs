namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class BaseUndoBehaviour : ScriptableObject
	{
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
				if (lastMove.type == GameMove.MoveType.CardMoved && gameMoves.Peek().type == GameMove.MoveType.CardMoved)
					await Awaitable.WaitForSecondsAsync(0.05f);

				await UndoMove(gameMoves);
			}
		}

		public abstract void UndoCardFlipped(GameMove.CardFlippedData flippedData);
		public abstract void UndoCardMoved(GameMove.CardMovedData movedData);
	}
}