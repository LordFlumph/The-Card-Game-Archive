namespace CardGameArchive.Behaviours
{
	
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;
	public abstract class BaseMoveBehaviour : ScriptableObject
	{
		[SerializeField] protected MoveSelector bestMoveChooser;

		[SerializeField] bool ignoreAutoMoveRestrictions;

		[SerializeField] float moveSpeed = -1;
		protected bool CanAutoMove => ignoreAutoMoveRestrictions || SettingsManager.Instance.AutoMoveCards;

		protected virtual List<ZoneParent> GetPossibleMoves(Card card, List<ZoneParent> allParents, bool simulation = false)
		{
			List<ZoneParent> possibleMoves = new();
			foreach (ZoneParent parent in allParents)
			{
				if (BaseGameRules.ActiveRules.IsMoveValid(card, parent, simulation))
					possibleMoves.Add(parent);
			}

			return possibleMoves;
		}

		public virtual List<ZoneParent> GetPossibleMoves(Card card, bool simulation = false)
		{
			if (card == null)
				return null;

			List<ZoneParent> allParents = GameBoard.Instance.AllZoneParents;
			return GetPossibleMoves(card, allParents, simulation);
		}

		public virtual List<ZoneParent> GetPossibleMoves(Card card, List<GameBoard.CardZone> validZones, bool simulation = false)
		{
			List<ZoneParent> possibleParents = new();
			foreach (GameBoard.CardZone zone in validZones)
			{
				possibleParents.AddRange(GameBoard.Instance.GetZoneParents(zone));
			}
			return GetPossibleMoves(card, possibleParents, simulation);
		}
		public virtual List<ZoneParent> GetPossibleMoves(Card card, GameBoard.CardZone validZone, bool simulation = false) => GetPossibleMoves(card, new List<GameBoard.CardZone>() { validZone }, simulation);

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
			if (!CanAutoMove)
				return;

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