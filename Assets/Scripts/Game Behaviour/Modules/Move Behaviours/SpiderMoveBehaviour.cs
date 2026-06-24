namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "SpiderMoveBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Move Behaviours/Spider")]
	public class SpiderMoveBehaviour : BaseMoveBehaviour
	{
		public override async void AutoMove()
		{
			foreach (Card card in GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau).Select(o => o.BottomCard).Where(o => o != null))
			{
				if (card.Rank == Card.CardRank.Ace)
				{
					List<Card> cardChain = BaseGameRules.ActiveRules.GetCardChain(card);
					if (cardChain?.Count == 13)
					{
						foreach (ZoneParent foundation in GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation))
						{
							if (foundation.CardCount == 0)
							{
								cardChain.Reverse();
								foreach (Card chainCard in cardChain)
								{
									await Awaitable.WaitForSecondsAsync(0.05f);
									GameTaskManager.Instance.AddTask(RunAutoMove(chainCard, foundation));
									chainCard.SetInteractable(false, false);
								}

								return;
							}
						}
						Debug.LogError("Attempting to move a complete stack to foundation but no foundation remains");
					}
				}
			}
		}
	}

}