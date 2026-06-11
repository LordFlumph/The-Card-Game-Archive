namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "ClockGameInputBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Game Input Behaviours/Clock")]
	public class ClockGameInputBehaviour : StandardGameInputBehaviour
	{
		protected override void OnCardDropped(Card card)
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
					if (GameBoard.Instance.GetZoneIndex(zoneParent) == BaseGameRules.ActiveRules.GetRankValue(card.Rank) - 1)
					{
						ZoneParent targetParent = zoneParent;
						if (zoneParent.Zone == GameBoard.CardZone.Tableau)
						{
							targetParent = GetLinkedZone(targetParent);
						}

						actionExecuted = true;
						GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, targetParent, timeToMove: 0.1f));
					}
					break;
				}

				else if (hit.collider.TryGetComponent(out CardObject otherCard))
				{
					ZoneParent otherZoneParent = otherCard.GetZoneParent();
					if (GameBoard.Instance.GetZoneIndex(otherZoneParent) == BaseGameRules.ActiveRules.GetRankValue(card.Rank) - 1)
					{
						if (otherZoneParent.Zone == GameBoard.CardZone.Tableau)
							otherZoneParent = GetLinkedZone(otherZoneParent);


						actionExecuted = true;
						GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, otherZoneParent, timeToMove: 0.1f));
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

		ZoneParent GetLinkedZone(ZoneParent parent) => GameBoard.Instance.GetZoneParents(parent.Zone == GameBoard.CardZone.Tableau
			? GameBoard.CardZone.Foundation : GameBoard.CardZone.Tableau)[GameBoard.Instance.GetZoneIndex(parent)];
	}
}
