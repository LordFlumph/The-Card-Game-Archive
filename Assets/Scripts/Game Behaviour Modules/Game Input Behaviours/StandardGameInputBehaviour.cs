namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	

	[CreateAssetMenu(fileName = "StandardGameInputBehaviour", menuName = "Card Game Archive/Game Behaviour/Game Input Behaviours/Standard")]
	public class StandardGameInputBehaviour : BaseGameInputBehaviour
	{
		public override void OnCardTapped(Card card)
		{
			GameTaskManager.Instance.AddTask(StandardGameManager.Instance.MoveCardToBestDestination(card));
		}
		public override void OnCardDropped(Card card)
		{
			bool actionExecuted = false;

			List<GameObject> objectsToIgnore = new();
			objectsToIgnore.AddRange(BaseGameRules.ActiveRules.GetCardChain(card).Select(o => o.linkedObj.gameObject));

			RaycastHit2D[] hits = Physics2D.RaycastAll(card.linkedObj.transform.position, Vector3.forward, GameBoard.TopCardZ * 2);
			foreach (var hit in hits.OrderBy(o => o.distance))
			{
				if (objectsToIgnore.Contains(hit.collider.gameObject))
					continue;

				if (hit.collider.TryGetComponent(out ZoneParent zoneParent))
				{
					if (zoneParent.CardCount == 0 && zoneParent != card.GetZoneParent())
					{
						if (BaseGameRules.ActiveRules.IsMoveValid(card, zoneParent))
						{
							actionExecuted = true;
							GameBoard.Instance.MoveCard(card, zoneParent);
						}
					}
					break;
				}

				else if (hit.collider.TryGetComponent(out CardObject otherCard))
				{
					ZoneParent otherZoneParent = otherCard.GetZoneParent();
					if (otherCard.Data == otherZoneParent.BottomCard)
					{
						if (BaseGameRules.ActiveRules.IsMoveValid(card, otherZoneParent))
						{
							actionExecuted = true;
							GameBoard.Instance.MoveCard(card, otherZoneParent);
						}
					}
					break;
				}
			}

			// No point in telling the player it was invalid if they clearly weren't trying to put it somewhere
			if (!actionExecuted && hits.Length > 0)
			{
				StandardGameManager.Instance.InvokeInvalidAction(card);
			}
		}
	}
}
