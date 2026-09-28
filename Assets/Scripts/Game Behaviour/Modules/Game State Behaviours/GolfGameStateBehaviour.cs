namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "GolfGameStateBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Game State Behaviours/Golf")]
	public class GolfGameStateBehaviour : BaseGameStateBehaviour
	{
		public override bool IsGameStuck()
		{
			if (GameBoard.Instance.GetZoneParent(GameBoard.CardZone.Stock).Cards.Count > 0)
				return false;

			List<Card> interactableCards = GameBoard.Instance.AllCards.Where(card => card.Interactable).ToList();

			foreach (Card card in interactableCards)
			{
				if (BaseGameRules.ActiveRules.IsMoveValid(card, GameBoard.Instance.GetZoneParent(GameBoard.CardZone.Foundation)))
					return false;
			}

			return true;
		}
	}

}