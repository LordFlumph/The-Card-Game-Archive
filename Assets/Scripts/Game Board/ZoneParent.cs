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

        [SerializeField] int maxCards = -1;

        [SerializeField] bool squishCards = false;
        [SerializeField] int cardsBeforeSquish = 0;
		[SerializeField] float timeToSquish;

		[SerializeField] bool coverCards = false;
        [SerializeField] int coverLimit = 0;
		[SerializeField] float timeToCover;


		public async Task PlaceCard(Card card, float timeToMove, bool teleport = false)
        {
			if (!childCards.Contains(card.linkedObj))
				childCards.Add(card.linkedObj);

			Vector3 offset = PositionOffset;

			if (childCards.Count >= 2)
			{
				card.linkedObj.transform.SetParent(childCards[^2].transform);
			}
			else
			{
				card.linkedObj.transform.SetParent(transform);
				offset.x = 0; offset.y = 0;
			}	

			if (teleport)
			{
				card.linkedObj.transform.localPosition = offset;
			}
			else
			{
				await MoveCard(card.linkedObj.gameObject, offset, timeToMove);
			}

			if (coverCards)
				HandleCover();

			if (squishCards)
				HandleSquish();
		}

		public void RemoveCard(Card card)
		{
			childCards.Remove(card.linkedObj);

			if (squishCards)
				HandleSquish();
		}

		void HandleCover()
		{
			if (childCards.Count <= 0)
				return;

			if (childCards.Count > coverLimit)
			{
				for (int i = 0; i <= childCards.Count - coverLimit; i++)
				{
					MoveCard(childCards[i].gameObject, new Vector3(0, 0, PositionOffset.z), timeToCover);
				}
			}
			else
			{
				MoveCard(childCards[0].gameObject, new Vector3(0, 0, PositionOffset.z), timeToCover);

				for (int i = 1; i < childCards.Count - coverLimit; i++)
				{
					MoveCard(childCards[i].gameObject, PositionOffset, timeToCover);
				}
			}			
		}

		void HandleSquish()
		{
			// Do we want to squish cards or scale them?
		}

		async Task MoveCard(GameObject obj, Vector3 targetLocalPosition, float timeToMove)
		{
			if (timeToMove <= 0)
			{
				obj.transform.localPosition = targetLocalPosition;
				return;
			}

			float moveSpeed = Vector3.Distance(obj.transform.localPosition, targetLocalPosition) / timeToMove;
			while (Vector3.Distance(obj.transform.localPosition, targetLocalPosition) > moveSpeed * 0.01f)
			{
				obj.transform.localPosition = Vector3.MoveTowards(obj.transform.localPosition, targetLocalPosition, moveSpeed * Time.deltaTime);
				await Task.Yield();
			}
			obj.transform.localPosition = targetLocalPosition;
		}
	} 
}
