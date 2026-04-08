namespace CardGameArchive
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System;
	using System.Linq;

	public class GameBoard : MonoBehaviour, ISaveable
	{
		public static GameBoard Instance { get; private set; }

		public const float TopCardZ = -15;

		[SerializeField] private GameObject cardPrefab;

		[SerializeField] private List<ZoneParent> stockParents;
		[SerializeField] private List<ZoneParent> wasteParents;
		[SerializeField] private List<ZoneParent> foundationParents;
		[SerializeField] private List<ZoneParent> tableauParents;
		public List<ZoneParent> AllZoneParents
		{
			get
			{
				List<ZoneParent> allParents = new();
				allParents.AddRange(stockParents);
				allParents.AddRange(wasteParents);
				allParents.AddRange(foundationParents);
				allParents.AddRange(tableauParents);
				return allParents;
			}
		}

		private List<Card> allCards = new();

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

		public void GenerateCards()
		{
			foreach (var stock in stockParents)
			{
				DeckObject deckObj = stock.GetComponent<DeckObject>();
				if (deckObj != null)
				{
					foreach (Card card in deckObj.Data.Cards)
					{
						MoveCard(card, stock, teleport: true, canUndo: false, affectCardChain: false);
					}
				}
			}

			allCards = allCards.OrderBy(o => o.ID).ToList();
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
				card.linkedObj = Instantiate(cardPrefab, stockParents[0].transform).GetComponent<CardObject>();
				card.linkedObj.InitialiseCard(card);
				allCards.Add(card);
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

			card.linkedObj.transform.position = card.linkedObj.transform.position.xy(TopCardZ);

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
		public int GetZoneIndex(ZoneParent zone)
		{
			if (stockParents.Contains(zone))
				return stockParents.IndexOf(zone);
			if (wasteParents.Contains(zone))
				return wasteParents.IndexOf(zone);
			if (foundationParents.Contains(zone))
				return foundationParents.IndexOf(zone);
			if (tableauParents.Contains(zone))
				return tableauParents.IndexOf(zone);

			return -1;
		}
		public List<Card> GetCardChain(ZoneParent zone)
		{
			if (zone.transform.childCount > 0)
			{
				return GetCardChain(zone.BottomCard);
			}

			return new();
		}

		public List<Card> GetCardChain(Card card, bool ascending = true)
		{
			if (card?.Data == null)
			{
				return new();
			}
			ZoneParent zoneParent = card.GetZoneParent();
			Card activeCard = card;
			while (zoneParent.TryGetNextCard(activeCard, out Card newCard))
			{
				if ((BaseGameManager.Instance.Rules.GetRankValue(activeCard.Rank) -
						BaseGameManager.Instance.Rules.GetRankValue(newCard.Rank)) == (ascending ? 1 : -1))
				{
					activeCard = newCard;
				}
				else
				{
					break;
				}
			}

			List<Card> cardChain = new();
			cardChain.Add(activeCard);

			while (zoneParent.TryGetPreviousCard(activeCard, out Card newCard))
			{
				if (!newCard.Flipped)
				{
					break;
				}

				if ((BaseGameManager.Instance.Rules.GetRankValue(activeCard.Rank) -
					BaseGameManager.Instance.Rules.GetRankValue(newCard.Rank)) == (ascending ? -1 : 1))
				{
					cardChain.Add(newCard);
					activeCard = newCard;
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

		public Card GetCardByID(int ID) => allCards.Find(card => card.ID == ID);

		public Deck GetDeck(int index = 0)
		{
			if (index < 0 || index >= stockParents.Count)
				return null;

			return stockParents[index].GetComponent<DeckObject>().Data;
		}

		public class BoardSaveData : SaveData
		{
			public List<SaveData> cardData = new();
		}

		public SaveData Save()
		{
			BoardSaveData data = new();

			foreach (Card card in allCards)
			{
				data.cardData.Add(card.linkedObj.Save());
			}

			return data;
		}

		public void Load(SaveData saveData)
		{
			BoardSaveData data = saveData as BoardSaveData;
			// Create all cards (Setting cards based on the ID)
			GenerateCards();
			List<CardObject.CardSaveData> cardsaveData = data.cardData.Cast<CardObject.CardSaveData>().ToList();

			foreach (Card card in allCards)
			{

			}
			// Move all cards to their correct positions
			// Load each ZoneParent's data
		}
	}
}