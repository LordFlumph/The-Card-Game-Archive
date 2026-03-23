namespace CardGameArchive
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public class GameBoard : MonoBehaviour
	{
		public static GameBoard Instance { get; private set; }

		[SerializeField] private GameObject cardPrefab;

		[SerializeField] private List<Transform> stockParents;
		[SerializeField] private List<Transform> wasteParents;
		[SerializeField] private List<Transform> foundationParents;
		[SerializeField] private List<Transform> tableauParents;

		[SerializeField] private Vector3 cardStockOffset;
		[SerializeField] private Vector3 cardWasteOffset;
		[SerializeField] private Vector3 cardFoundationOffset;
		[SerializeField] private Vector3 cardTableauOffset;

		[SerializeField] private float cardMoveSpeed;


		public enum CardDestination
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

		public async void PlaceCard(Card card, CardDestination destination,
									int index = -1,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false)
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
				card.linkedObj = Instantiate(cardPrefab);
			}

			List<Transform> targetList = new();
			Vector3 targetOffset = Vector3.zero;

			switch (destination)
			{
				case CardDestination.Stock:	
					if (index >= stockParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.interactable = setInteractable;
						return;
					}

					targetList = stockParents;
					targetOffset = cardStockOffset;
					break;
				case CardDestination.Waste:
					if (index >= wasteParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.interactable = setInteractable;
						return;
					}

					targetList = wasteParents;
					targetOffset = cardWasteOffset;
					break;
				case CardDestination.Foundation:
					if (index >= foundationParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.interactable = setInteractable;
						return;
					}

					targetList = foundationParents;
					targetOffset = cardFoundationOffset;
					break;
				case CardDestination.Tableau:
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


			card.linkedObj.transform.SetParent(targetList[index]);

			if (teleport)
			{
				card.linkedObj.transform.localPosition = targetOffset * (targetList[index].childCount-1);
			}
			else
			{
				if (fromStock)
				{
					card.linkedObj.transform.position = stockParents[stockIndex].transform.position;
				}

				await MoveCard(card.linkedObj, targetOffset * (targetList[index].childCount-1));
			}

			card.interactable = setInteractable;
		}

		async Task MoveCard(GameObject obj, Vector3 targetLocalPosition)
		{
			while (Vector3.Distance(obj.transform.localPosition, targetLocalPosition) > cardMoveSpeed * 0.1f)
			{
				obj.transform.localPosition = Vector3.MoveTowards(obj.transform.localPosition, targetLocalPosition, cardMoveSpeed * Time.deltaTime);
				await Task.Yield();
			}
			obj.transform.localPosition = targetLocalPosition;
		}
	}
}