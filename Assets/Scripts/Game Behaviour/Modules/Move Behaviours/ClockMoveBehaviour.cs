namespace CardGameArchive.Behaviours
{
	using System.Linq;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "ClockMoveBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Move Behaviours/Clock")]
	public class ClockMoveBehaviour : BaseMoveBehaviour
	{
		public override async void AutoMove()
		{
			Card activeCard = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau).FirstOrDefault(o => o.CardCount > 0 && o.BottomCard.Interactable)?.BottomCard;

			if (activeCard == null)
				return;

			if (activeCard.Rank.ToString().ToLower() == activeCard.GetZoneParent().name.ToLower())
			{
				activeCard.SetInteractable(false, false);
				GameTaskManager.Instance.AddTask(Task.Delay(100));
				ZoneParent destination = GameBoard.Instance.GetZoneParent(GameBoard.CardZone.Foundation, BaseGameRules.ActiveRules.GetRankValue(activeCard.Rank) - 1);
				GameTaskManager.Instance.QueueTask(() => RunAutoMove(activeCard, destination));
			}
		}

		public override async Task MoveCardToBestDestination(Card card, bool playerDriven = true)
		{
			card.SetInteractable(false, false);
			GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, destination: GameBoard.CardZone.Foundation, index: BaseGameRules.ActiveRules.GetRankValue(card.Rank) - 1));
		}
	}

}