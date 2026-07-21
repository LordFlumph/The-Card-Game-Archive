namespace CardGameArchive.Behaviours
{
    using UnityEngine;

	public class PyramidMoveBehaviour : BaseMoveBehaviour
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
	}
}
