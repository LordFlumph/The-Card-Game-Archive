namespace CardGameArchive
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System;

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

		public struct CardMoveEvent
		{
			public Card card;
			public ZoneParent from;
			public ZoneParent to;
			public bool teleport;
			public bool canUndo;

			public CardMoveEvent(Card card, ZoneParent from, ZoneParent to, bool teleport, bool canUndo)
			{
				this.card = card;
				this.from = from;
				this.to = to;
				this.teleport = teleport;
				this.canUndo = canUndo;
			}
		}
		public event Action<CardMoveEvent> OnCardMoveStart;
		public event Action<CardMoveEvent> OnCardMoveFinish;

		public enum CardZone
		{
			Stock,
			Waste,
			Foundation,
			Tableau,
		}

		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		public async Task MoveCard(Card card, ZoneParent destination,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,
									float timeToMove = -1,
									bool canUndo = true,
									bool affectCardChain = true)
		{
			switch (destination.Zone)
			{
				case CardZone.Stock:
					await MoveCard(card, CardZone.Stock, stockParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove, canUndo, affectCardChain);
					break;
				case CardZone.Waste:
					await MoveCard(card, CardZone.Waste, wasteParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove, canUndo, affectCardChain);
					break;
				case CardZone.Foundation:
					await MoveCard(card, CardZone.Foundation, foundationParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove, canUndo, affectCardChain);
					break;
				case CardZone.Tableau:
					await MoveCard(card, CardZone.Tableau, tableauParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove, canUndo, affectCardChain);
					break;
			}
		}
		public async Task MoveCard(CardObject card, ZoneParent destination,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,
									float timeToMove = -1,
									bool canUndo = true,
									bool affectCardChain = true)
		{
			await MoveCard(card.Data, destination, fromStock, stockIndex, teleport, timeToMove, canUndo, affectCardChain);
		}

		public async Task MoveCard(Card card, CardZone destination,
									int index = 0,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,
									float timeToMove = -1,
									bool canUndo = true,
									bool affectCardChain = true)
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

			ZoneParent targetZone = null;

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

			ZoneParent originalZone = card.linkedObj.GetZoneParent();

			if (originalZone != null)
			{
				originalZone.RemoveCard(card, affectCardChain);
			}

			CardMoveEvent moveEventData = new(card, originalZone, targetZone, teleport, canUndo);
			OnCardMoveStart?.Invoke(moveEventData);

			await targetZone.PlaceCard(card, timeToMove, teleport, affectCardChain);

			card.SetInteractable(setInteractable);

			OnCardMoveFinish?.Invoke(moveEventData);
		}

		public async Task MoveCard(CardObject card, CardZone destination,
									int index = 0,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,
									float timeToMove = -1,
									bool canUndo = true,
									bool affectCardChain = true)
		{
			await MoveCard(card.Data, destination, index, fromStock, stockIndex, teleport, timeToMove, canUndo, affectCardChain);
		}

		public List<ZoneParent> GetZoneParents(CardZone zone)
		{
			return zone switch
			{
				CardZone.Stock => new(stockParents),
				CardZone.Waste => new(wasteParents),
				CardZone.Foundation => new(foundationParents),
				CardZone.Tableau => new(tableauParents),
				_ => throw new NotImplementedException()
			};
		}

		public List<Card> GetCardChain(ZoneParent zone)
		{
			if (zone.transform.childCount > 0)
			{
				return GetCardChain(zone.transform.GetBottomChild().GetComponent<CardObject>());
			}

			return new();
		}

		public List<Card> GetCardChain(CardObject card, bool ascending = true)
		{
			if (card?.Data == null)
			{
				return new();
			}

			CardObject activeCard = card;
			while (activeCard.transform.childCount > 0)
			{
				if (activeCard.transform.GetChild(0).TryGetComponent(out CardObject newCard))
				{
					if ((BaseGameManager.Instance.Rules.GetRankValue(activeCard.Data.Rank) -
						BaseGameManager.Instance.Rules.GetRankValue(newCard.Data.Rank)) == (ascending ? 1 : -1))
					{
						activeCard = newCard;
					}
					else
					{
						break;
					}

				}
				else
				{
					break;
				}
			}

			List<Card> cardChain = new();
			cardChain.Add(activeCard.Data);

			while (activeCard.transform.parent != null)
			{
				if (activeCard.transform.parent.TryGetComponent(out CardObject newCard))
				{
					if (!newCard.Data.Flipped)
					{
						break;
					}

					if ((BaseGameManager.Instance.Rules.GetRankValue(activeCard.Data.Rank) -
						BaseGameManager.Instance.Rules.GetRankValue(newCard.Data.Rank)) == (ascending ? -1 : 1))
					{
						cardChain.Add(newCard.Data);
						activeCard = newCard;
					}
					else
					{
						break;
					}
					
				}	
				else
				{
					break;
				}
			}

			// Finally, reverse the card chain (since we want it to be from the first card in the chain down
			cardChain.Reverse();
			return cardChain;
		}	
	}
}