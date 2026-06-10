namespace CardGameArchive.Behaviours
{
	using System.Linq;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "ClockMoveBehaviour", menuName = "Card Game Archive/Game Behaviour/Move Behaviours/Clock")]
	public class ClockMoveBehaviour : BaseMoveBehaviour
	{
		public override async void AutoMove()
		{
			Card activeCard = GameBoard.Instance.AllCards.First(o => o.Interactable);

			if (activeCard.Rank.ToString().ToLower() == activeCard.GetZoneParent().name.ToLower())
			{
				GameTaskManager.Instance.AddTask(Task.Delay(100));
				GameTaskManager.Instance.QueueTask(() => GameBoard.Instance.MoveCard(activeCard, destination: GameBoard.CardZone.Foundation, index: BaseGameRules.ActiveRules.GetRankValue(activeCard.Rank) - 1, timeToMove: 0.1f));
			}
		}

		public override async Task MoveCardToBestDestination(Card card, bool playerDriven = true)
		{
			GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, destination: GameBoard.CardZone.Foundation, index: BaseGameRules.ActiveRules.GetRankValue(card.Rank) - 1));
		}
	}

}