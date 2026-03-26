namespace CardGameArchive
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Mono.Cecil;
    using System.Linq;

    public class GameBoard : MonoBehaviour
	{
		public static GameBoard Instance { get; private set; }

		[SerializeField] private GameObject cardPrefab;

		[SerializeField] private List<ZoneParent> stockParents;
		[SerializeField] private List<ZoneParent> wasteParents;
		[SerializeField] private List<ZoneParent> foundationParents;
		[SerializeField] private List<ZoneParent> tableauParents;

		[field: SerializeField] public Vector3 CardStockOffset { get; private set; }
		[field: SerializeField] public Vector3 CardWasteOffset { get; private set; }
		[field: SerializeField] public Vector3 CardFoundationOffset { get; private set; }
		[field: SerializeField] public Vector3 CardTableauOffset { get; private set; }

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

		public async void PlaceCard(Card card, ZoneParent destination,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,									
									float timeToMove = -1)
		{
			switch (destination.Zone)
			{
				case CardZone.Stock:
				await PlaceCard(card, CardZone.Stock, stockParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove);
				break;
				case CardZone.Waste:
				break;
				case CardZone.Foundation:
				break;
				case CardZone.Tableau:
				break;
			}
		}

		public async Task PlaceCard(Card card, CardZone destination,
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

			if (card.linkedObj == null)
			{
				card.linkedObj = Instantiate(cardPrefab, stockParents[0].transform.position, Quaternion.identity).GetComponent<CardObject>();
				card.linkedObj.InitialiseCard(card);
			}

			bool setInteractable = card.Interactable;
			card.SetInteractable(false);

			if (timeToMove <= 0)
				timeToMove = cardMoveTime;

			ZoneParent targetZone = new();

			switch (destination)
			{
				case CardZone.Stock:	
					if (index >= stockParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.SetInteractable(setInteractable);
						return;
					}

					targetZone = stockParents[index];
					break;
				case CardZone.Waste:
					if (index >= wasteParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.SetInteractable(setInteractable);
						return;
					}

					targetZone = wasteParents[index];
					break;
				case CardZone.Foundation:
					if (index >= foundationParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.SetInteractable(setInteractable);
						return;
					}

					targetZone = foundationParents[index];
					break;
				case CardZone.Tableau:
					if (index >= tableauParents.Count)
					{
						Debug.LogError($"Invalid index");
						card.SetInteractable(setInteractable);
						return;
					}

					targetZone = tableauParents[index];
					break;
			}

			if (fromStock)
			{
				card.linkedObj.transform.position = stockParents[stockIndex].transform.position;
			}

			card.linkedObj.GetComponentInParent<ZoneParent>()?.RemoveCard(card);

			await targetZone.PlaceCard(card, timeToMove, teleport);

			card.SetInteractable(setInteractable);
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