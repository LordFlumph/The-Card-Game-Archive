namespace CardGameArchive
{
	using System;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using UnityEngine;

	// This will handle card squishing, coverring and placing from now on
	public class ZoneParent : MonoBehaviour
	{
		[field: SerializeField] public GameBoard.CardZone Zone { get; private set; }

		Vector3 PositionOffset
		{
			get
			{
				return Zone switch
				{
					GameBoard.CardZone.Stock => GameBoard.Instance.CardStockOffset,
					GameBoard.CardZone.Waste => GameBoard.Instance.CardWasteOffset,
					GameBoard.CardZone.Foundation => GameBoard.Instance.CardFoundationOffset,
					GameBoard.CardZone.Tableau => GameBoard.Instance.CardTableauOffset,
					_ => throw new NotImplementedException()
				};
			}
		}

		List<CardObject> childCards = new();
		public int CardCount => childCards.Count;

		[SerializeField] int maxCards = -1;

		public bool ZoneFull => maxCards != -1 && CardCount >= maxCards;

		[SerializeField] bool squishCards = false;
		[SerializeField] int cardsBeforeSquish = 0;
		[SerializeField] float timeToSquish;

		[SerializeField] bool coverCards = false;
		[SerializeField] int coverLimit = 0;
		[SerializeField] float timeToCover;

		bool useOperations = false;

		public Card BottomCard
		{
			get
			{
				if (childCards.Count > 0)
					return childCards[^1].CardData;
				return null;
			}
		}


		public async Task PlaceCard(Card card, float timeToMove, bool teleport = false, bool addLowerChain = true)
		{
			Vector3 offset = PositionOffset;

			if (childCards.Count >= 1)
			{
				card.linkedObj.transform.SetParent(childCards[^1].transform);
			}
			else
			{
				card.linkedObj.transform.SetParent(transform);
				offset.x = 0; offset.y = 0;
			}

			if (addLowerChain)
			{
				List<Card> cardChain = GameBoard.Instance.GetCardChain(card.linkedObj);
				for (int i = cardChain.IndexOf(card); i < cardChain.Count; i++)
				{
					if (!childCards.Contains(cardChain[i].linkedObj))
						childCards.Add(cardChain[i].linkedObj);
				}
			}
			else
			{
				if (!childCards.Contains(card.linkedObj))
					childCards.Add(card.linkedObj);
			}

			if (teleport)
			{
				card.linkedObj.transform.localPosition = offset;
			}
			else
			{
				await MoveCard(card.linkedObj, offset, timeToMove);
			}

			if (coverCards)
				HandleCover();

			if (squishCards)
				HandleSquish();
		}

		public void RemoveCard(Card card, bool removeLowerChain = true)
		{
			if (removeLowerChain)
			{
				List<Card> cardChain = GameBoard.Instance.GetCardChain(card.linkedObj);
				for (int i = cardChain.IndexOf(card); i < cardChain.Count; i++)
				{
					if (childCards.Contains(cardChain[i].linkedObj))
						childCards.Remove(cardChain[i].linkedObj);
				}
			}
			else
			{
				if (childCards.Contains(card.linkedObj))
					childCards.Remove(card.linkedObj);
			}

			if (coverCards)
				HandleCover();

			if (squishCards)
				HandleSquish();
		}

		public void SetOperations(bool useOperations)
		{
			this.useOperations = useOperations;

			if (useOperations)
			{
				HandleCover();
				HandleSquish();
			}
		}

		void HandleCover()
		{
			if (!useOperations)
				return;

			if (!coverCards || childCards.Count <= 0)
				return;

			if (childCards.Count > coverLimit)
			{
				for (int i = 0; i <= childCards.Count - coverLimit; i++)
				{
					Vector3 newOffset = new Vector3(0, 0, PositionOffset.z);

					if (Vector3.Distance(childCards[i].transform.localPosition, newOffset) > 0.01f)
						MoveCard(childCards[i], newOffset, timeToCover);
				}

				for (int i = childCards.Count - coverLimit + 1; i < childCards.Count; i++)
				{
					if (Vector3.Distance(childCards[i].transform.localPosition, PositionOffset) > 0.01f)
						MoveCard(childCards[i], PositionOffset, timeToCover);
				}
			}
			else
			{
				for (int i = 1; i < childCards.Count; i++)
				{
					if (Vector3.Distance(childCards[i].transform.localPosition, PositionOffset) > 0.01f)
						MoveCard(childCards[i], PositionOffset, timeToCover);
				}
			}
		}

		void HandleSquish()
		{
			if (!useOperations)
				return;

			if (!squishCards || childCards.Count <= 0)
				return;

			// Do we want to squish cards or scale them?
		}

		// TODO: Rework this so that it uses a single targetPosition reference. Probably on CardObject
		async Task MoveCard(CardObject obj, Vector3 targetLocalPosition, float timeToMove)
		{
			if (timeToMove <= 0)
			{
				obj.transform.localPosition = targetLocalPosition;
				return;
			}

			float moveSpeed = Vector3.Distance(obj.transform.localPosition, targetLocalPosition) / timeToMove;
			moveSpeed = Mathf.Max(moveSpeed, 0.1f);
			while (Vector3.Distance(obj.transform.localPosition, targetLocalPosition) > moveSpeed * 0.01f)
			{
				obj.transform.localPosition = Vector3.MoveTowards(obj.transform.localPosition, targetLocalPosition, moveSpeed * Time.deltaTime);
				await Task.Yield();
			}
			obj.transform.localPosition = targetLocalPosition;
		}
	}
}
