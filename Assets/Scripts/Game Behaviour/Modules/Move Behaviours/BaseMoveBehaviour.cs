namespace CardGameArchive.Behaviours
{
	
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;
	public abstract class BaseMoveBehaviour : BaseBehaviour
	{
		[SerializeField] protected MoveSelector bestMoveChooser;

		[SerializeField] bool ignoreAutoMoveRestrictions;

		[SerializeField] float moveSpeed = -1;
		protected bool CanAutoMove => ignoreAutoMoveRestrictions || SettingsManager.Instance.AutoMoveCards;

		public enum CheatType
		{ 
			NONE,
			MoveAnywhere
		}
		[SerializeField] CheatType cheatType = CheatType.NONE;

		protected virtual List<ZoneParent> GetPossibleMoves(Card card, List<ZoneParent> allParents, BaseGameRules.MoveValidationMode mode = BaseGameRules.MoveValidationMode.Standard)
		{
			List<ZoneParent> possibleMoves = new();
			foreach (ZoneParent parent in allParents)
			{
				if (BaseGameRules.ActiveRules.IsMoveValid(card, parent, mode))
					possibleMoves.Add(parent);
			}

			return possibleMoves;
		}

		public virtual List<ZoneParent> GetPossibleMoves(Card card, BaseGameRules.MoveValidationMode mode = BaseGameRules.MoveValidationMode.Standard)
		{
			if (card == null)
				return null;

			List<ZoneParent> allParents = GameBoard.Instance.AllZoneParents;
			return GetPossibleMoves(card, allParents, mode);
		}

		public virtual List<ZoneParent> GetPossibleMoves(Card card, List<GameBoard.CardZone> validZones, BaseGameRules.MoveValidationMode mode = BaseGameRules.MoveValidationMode.Standard)
		{
			List<ZoneParent> possibleParents = new();
			foreach (GameBoard.CardZone zone in validZones)
			{
				possibleParents.AddRange(GameBoard.Instance.GetZoneParents(zone));
			}
			return GetPossibleMoves(card, possibleParents, mode);
		}
		public virtual List<ZoneParent> GetPossibleMoves(Card card, GameBoard.CardZone validZone, BaseGameRules.MoveValidationMode mode = BaseGameRules.MoveValidationMode.Standard) 
			=> GetPossibleMoves(card, new List<GameBoard.CardZone>() { validZone }, mode);

		/// <summary>
		/// Automatically find and move a card
		/// </summary>
		public abstract void AutoMove();

		/// <summary>
		/// Automatically move this card to the best calculated destination for it, as decided by the bestMoveChooser
		/// </summary>
		/// <param name="card"></param>
		/// <param name="playerDriven">Whether this move was triggered directly by the player</param>
		/// <returns></returns>
		public virtual async Task MoveCardToBestDestination(Card card, bool playerDriven = true)
		{
			ZoneParent bestMoveTarget = bestMoveChooser.GetBestMove(GetPossibleMoves(card), card);

			if (bestMoveTarget == null)
			{
				if (playerDriven)
					StandardGameManager.Instance.InvokeInvalidAction(card);

				return;
			}

			Task moving = GameBoard.Instance.MoveCard(card, bestMoveTarget);
			GameTaskManager.Instance.AddTask(moving);
			await moving;
		}

		protected async Task RunAutoMove(Card card, ZoneParent destination)
		{
			await GameBoard.Instance.MoveCard(card, destination, forceContingent: true, canUndo: StandardGameManager.Instance.CanUndo, timeToMove: moveSpeed);
		}
	}
}