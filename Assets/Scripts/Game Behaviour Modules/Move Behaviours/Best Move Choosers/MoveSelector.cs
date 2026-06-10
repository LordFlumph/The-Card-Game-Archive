namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "MoveSelector", menuName = "Card Game Archive/Game Behaviour/Move Behaviours/Move Selectors/Move Selector")]
	public class MoveSelector : ScriptableObject
	{
		public enum PriorityOption
		{
			LongestChain,
			ShortestChain,
			NonEmpty,
			Empty,
			MostVisible,
			LeastVisible,
			MostHidden,
			LeastHidden,
			MostCards,
			LeastCards
		}

		[Tooltip("The order in which zones will be prioritised when choosing a move. Any missing zones will be considered as lowest priority")]
		[SerializeField] List<GameBoard.CardZone> zonePriority;
		[SerializeField] List<PriorityOption> orderPriority;
		[Tooltip("When all filters have been applied, should the selected move be randomly selected from the last remaining moves? If false, the first move will be selected")]
		[SerializeField] bool selectRandomFromFinalists;

		public virtual ZoneParent GetBestMove(List<ZoneParent> allMoves)
		{
			if (allMoves == null || allMoves.Count == 0)
				return null;

			if (allMoves.Count == 1)
				return allMoves[0];

			// Narrow to all moves in highest priority zone
			List<ZoneParent> priorityMoves = new();
			foreach (GameBoard.CardZone zone in zonePriority)
			{
				if (allMoves.Any(o => o.Zone == zone))
				{
					priorityMoves.AddRange(allMoves.Where(o => o.Zone == zone));
					break;
				}
			}

			// No moves found in any of the priority zones, so consider all moves
			if (priorityMoves.Count == 0)
				priorityMoves.AddRange(allMoves);

			foreach (PriorityOption priority in orderPriority)
			{
				switch (priority)
				{
					case PriorityOption.LongestChain:
						int longestChain = priorityMoves.Max(o => BaseGameRules.ActiveRules.GetCardChain(o).Count);
						priorityMoves = priorityMoves.Where(o => BaseGameRules.ActiveRules.GetCardChain(o).Count == longestChain).ToList();
						break;
					case PriorityOption.ShortestChain:
						int shortestChain = priorityMoves.Min(o => BaseGameRules.ActiveRules.GetCardChain(o).Count);
						priorityMoves = priorityMoves.Where(o => BaseGameRules.ActiveRules.GetCardChain(o).Count == shortestChain).ToList();
						break;
					case PriorityOption.NonEmpty:
						if (priorityMoves.Any(o => o.CardCount > 0))
							priorityMoves = priorityMoves.Where(o => o.CardCount > 0).ToList();
						break;
					case PriorityOption.Empty:
						if (priorityMoves.Any(o => o.CardCount == 0))
							priorityMoves = priorityMoves.Where(o => o.CardCount == 0).ToList();
						break;
					case PriorityOption.MostVisible:
						int mostVisible = priorityMoves.Max(o => o.VisibleCards);
						priorityMoves = priorityMoves.Where(o => o.VisibleCards == mostVisible).ToList();
						break;
					case PriorityOption.LeastVisible:
						int leastVisible = priorityMoves.Min(o => o.VisibleCards);
						priorityMoves = priorityMoves.Where(o => o.VisibleCards == leastVisible).ToList();
						break;
					case PriorityOption.MostHidden:
						int mostHidden = priorityMoves.Max(o => o.HiddenCards);
						priorityMoves = priorityMoves.Where(o => o.HiddenCards == mostHidden).ToList();
						break;
					case PriorityOption.LeastHidden:
						int leastHidden = priorityMoves.Min(o => o.HiddenCards);
						priorityMoves = priorityMoves.Where(o => o.HiddenCards == leastHidden).ToList();
						break;
					case PriorityOption.MostCards:
						int mostCards = priorityMoves.Max(o => o.CardCount);
						priorityMoves = priorityMoves.Where(o => o.CardCount == mostCards).ToList();
						break;
					case PriorityOption.LeastCards:
						int leastCards = priorityMoves.Min(o => o.CardCount);
						priorityMoves = priorityMoves.Where(o => o.CardCount == leastCards).ToList();
						break;
				}

				if (priorityMoves.Count == 1)
					break;
			}

			return priorityMoves[selectRandomFromFinalists ? Random.Range(0, priorityMoves.Count) : 0];
		}
	}

}