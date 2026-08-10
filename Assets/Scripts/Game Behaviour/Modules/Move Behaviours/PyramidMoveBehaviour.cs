namespace CardGameArchive.Behaviours
{
    using UnityEngine;

	[CreateAssetMenu(fileName = "PyramidMoveBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Move Behaviours/Pyramid")]
	public class PyramidMoveBehaviour : BaseMultiselectMoveBehaviour
	{
		public override void AutoMove()
		{
			if (!SettingsManager.Instance.AutoMoveCards)
				return;

			foreach (ZoneParent parent in GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau))
			{
				if (parent.CardCount > 0 && parent.BottomCard.Rank == Card.CardRank.King && BaseGameRules.ActiveRules.CanCardMove(parent.BottomCard))
				{
					GameBoard.Instance.MoveCard(parent.BottomCard, GameBoard.CardZone.Foundation, forceContingent: true);
					return;
				}
			}

			ZoneParent waste = GameBoard.Instance.GetZoneParent(GameBoard.CardZone.Waste);
			if (waste.CardCount > 0 && waste.BottomCard.Rank == Card.CardRank.King && BaseGameRules.ActiveRules.CanCardMove(waste.BottomCard))
			{
				GameBoard.Instance.MoveCard(waste.BottomCard, GameBoard.CardZone.Foundation, forceContingent: true);
				return;
			}
		}

		protected override void OnSelect(CardSelectionRuntimeData selectionData)
		{
			if (selectionData.SelectedCardCount >= 2)
			{
				if (BaseGameRules.ActiveRules.GetRankValue(selectionData[0]) + BaseGameRules.ActiveRules.GetRankValue(selectionData[1]) == 13)
				{
					GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(selectionData[0], GameBoard.CardZone.Foundation));
					GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(selectionData[1], GameBoard.CardZone.Foundation, forceContingent: true));
				}
				else
				{
					FeedbackManager.Instance.OnInvalidAction(selectionData[0].Data);
					FeedbackManager.Instance.OnInvalidAction(selectionData[1].Data);
				}

				selectionData.DeselectAll();
			}
		}
	}
}
