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
		public List<CardObject> Cards { get { return new(childCards); } }
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
					return childCards[^1].Data;
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

			Task moving = card.linkedObj.MoveCard(offset, timeToMove, teleport);

			if (coverCards)
				HandleCover();

			if (squishCards)
				HandleSquish();

			await moving;
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
			this.useOperations = true;

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
						childCards[i].MoveCard(newOffset, timeToCover);
				}

				for (int i = childCards.Count - coverLimit + 1; i < childCards.Count; i++)
				{
					if (Vector3.Distance(childCards[i].transform.localPosition, PositionOffset) > 0.01f)
						childCards[i].MoveCard(PositionOffset, timeToCover);
				}
			}
			else
			{
				for (int i = 1; i < childCards.Count; i++)
				{
					if (Vector3.Distance(childCards[i].transform.localPosition, PositionOffset) > 0.01f)
						childCards[i].MoveCard(PositionOffset, timeToCover);
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
	}
}
