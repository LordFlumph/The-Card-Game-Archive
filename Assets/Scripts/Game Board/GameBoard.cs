namespace CardGameArchive
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Mono.Cecil;

	public class GameBoard : MonoBehaviour
	{
		public static GameBoard Instance { get; private set; }

		[SerializeField] private GameObject cardPrefab;

		[SerializeField] private List<ZoneParent> stockParents;
		[SerializeField] private List<ZoneParent> wasteParents;
		[SerializeField] private List<ZoneParent> foundationParents;
		[SerializeField] private List<ZoneParent> tableauParents;

		[SerializeField] private Vector3 cardStockOffset;
		[SerializeField] private Vector3 cardWasteOffset;
		[SerializeField] private Vector3 cardFoundationOffset;
		[SerializeField] private Vector3 cardTableauOffset;

		[SerializeField] private float cardMoveTime;

		public enum CardZone
		{
			Stock,
			Waste,
			Foundation,
			Tableau
		}

		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		public async void PlaceCard(Card card, CardZone destination,
									int index = 0,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,									
									float timeToMove = -1)
		{
			if (card == null)
			{
				Debug.LogError("Card is null");
				return;
			}
			
			if (fromStock && stockIndex < 0 || stockIndex >= stockParents.Count)
			{
				Debug.LogError("Invalid stock index");
			}

			bool setInteractable = card.interactable;
			card.interactable = false;

			if (card.linkedObj == null)
			{
				card.linkedObj = Instantiate(cardPrefab, stockParents[0].transform.position, Quaternion.identity).GetComponent<CardObject>();
				card.linkedObj.InitialiseCard(card);
			}

			if (timeToMove <= 0)
				timeToMove = cardMoveTime;

			List<ZoneParent> targetList = new();
			Vector3 targetOffset = Vector3.zero;

			switch (destination)
			{
				case CardZone.Stock:	
					if (index >= stockParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.interactable = setInteractable;
						return;
					}

					targetList = stockParents;
					targetOffset = cardStockOffset;
					break;
				case CardZone.Waste:
					if (index >= wasteParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.interactable = setInteractable;
						return;
					}

					targetList = wasteParents;
					targetOffset = cardWasteOffset;
					break;
				case CardZone.Foundation:
					if (index >= foundationParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.interactable = setInteractable;
						return;
					}

					targetList = foundationParents;
					targetOffset = cardFoundationOffset;
					break;
				case CardZone.Tableau:
					if (index >= tableauParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.interactable = setInteractable;
						return;
					}

					targetList = tableauParents;
					targetOffset = cardTableauOffset;
					break;
			}

			Vector3 targetPosition = targetOffset * (targetList[index].transform.childCount - 1);

			card.linkedObj.transform.SetParent(targetList[index].transform);

			if (teleport)
			{
				card.linkedObj.transform.localPosition = targetPosition;
			}
			else
			{
				if (fromStock)
				{
					card.linkedObj.transform.position = stockParents[stockIndex].transform.position;
				}

				await MoveCard(card.linkedObj.gameObject, targetPosition, timeToMove);
			}

			card.interactable = setInteractable;
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

		public List<ZoneParent> GetZoneParents(CardZone zone)
		{
			return zone switch
			{
				CardZone.Stock => stockParents,
				CardZone.Waste => wasteParents,
				CardZone.Foundation => foundationParents,
				CardZone.Tableau => tableauParents,
				_ => throw new System.NotImplementedException()
			};
		}
	}
}