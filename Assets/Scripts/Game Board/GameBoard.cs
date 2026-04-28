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
			public bool contingent;

			public CardMoveEvent(Card card, ZoneParent from, ZoneParent to, bool teleport, bool canUndo, bool contingent = false)
			{
				this.card = card;
				this.from = from;
				this.to = to;
				this.teleport = teleport;
				this.canUndo = canUndo;
				this.contingent	= contingent;
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

			NULL
		}

		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		public async Task GenerateCards()
		{
			List<Task> tasks = new();
			foreach (var stock in stockParents)
			{
				DeckObject deckObj = stock.GetComponent<DeckObject>();
				if (deckObj != null)
				{
					foreach (Card card in deckObj.Data.Cards)
					{
						tasks.Add(MoveCard(card, stock, true, stockParents.IndexOf(stock), teleport: true, canUndo: false, affectCardChain: false));
						card.SetInteractable(false);
					}
				}
			}

			allCards = allCards.OrderBy(o => o.ID).ToList();

			await Task.WhenAll(tasks);
		}

		public async Task MoveCard(Card card, ZoneParent destination,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,
									float timeToMove = -1,
									bool canUndo = true, bool forceContingent = false,
									bool affectCardChain = true)
		{
			switch (destination.Zone)
			{
				case CardZone.Stock:
					await MoveCard(card, CardZone.Stock, stockParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove, canUndo, forceContingent, affectCardChain);
					break;
				case CardZone.Waste:
					await MoveCard(card, CardZone.Waste, wasteParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove, canUndo, forceContingent, affectCardChain);
					break;
				case CardZone.Foundation:
					await MoveCard(card, CardZone.Foundation, foundationParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove, canUndo, forceContingent, affectCardChain);
					break;
				case CardZone.Tableau:
					await MoveCard(card, CardZone.Tableau, tableauParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove, canUndo, forceContingent, affectCardChain);
					break;
			}
		}

		public async Task MoveCard(CardObject card, ZoneParent destination,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,
									float timeToMove = -1,
									bool canUndo = true, bool forceContingent = false,
									bool affectCardChain = true)
		{
			await MoveCard(card.Data, destination, fromStock, stockIndex, teleport, timeToMove, canUndo, forceContingent, affectCardChain);
		}

		public async Task MoveCard(Card card, CardZone destination,
									int index = 0,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,
									float timeToMove = -1,
									bool canUndo = true, bool forceContingent = false,
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
				card.linkedObj = Instantiate(cardPrefab, stockParents[stockIndex].transform.position, Quaternion.identity).GetComponent<CardObject>();
				card.linkedObj.InitialiseCard(card);
				allCards.Add(card);
			}

			if (timeToMove <= 0)
				timeToMove = cardMoveTime;

			ZoneParent targetZone = null;

			switch (destination)
			{
				case CardZone.Stock:
					if (index >= stockParents.Count)
					{
						Debug.LogError($"Invalid index");
						return;
					}

					targetZone = stockParents[index];
					break;
				case CardZone.Waste:
					if (index >= wasteParents.Count)
					{
						Debug.LogError($"Invalid index");
						return;
					}

					targetZone = wasteParents[index];
					break;
				case CardZone.Foundation:
					if (index >= foundationParents.Count)
					{
						Debug.LogError($"Invalid index");
						return;
					}

					targetZone = foundationParents[index];
					break;
				case CardZone.Tableau:
					if (index >= tableauParents.Count)
					{
						Debug.LogError($"Invalid index");
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

			CardMoveEvent moveEventData = new(card, originalZone, targetZone, teleport, canUndo, forceContingent);
			OnCardMoveStart?.Invoke(moveEventData);

			card.linkedObj.transform.position = card.linkedObj.transform.position.xy(TopCardZ);

			if (destination != CardZone.NULL)
			{
				await targetZone.PlaceCard(card, timeToMove, teleport, affectCardChain);
			}
			else
			{
				card.linkedObj.transform.SetParent(null);
			}			

			OnCardMoveFinish?.Invoke(moveEventData);
		}

		public async Task MoveCard(CardObject card, CardZone destination,
									int index = 0,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,
									float timeToMove = -1,
									bool canUndo = true, bool forceContingent = false,
									bool affectCardChain = true)
		{
			await MoveCard(card.Data, destination, index, fromStock, stockIndex, teleport, timeToMove, canUndo, forceContingent, affectCardChain);
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
		

		//public List<Card> GetCardChain(Card card, bool ascending = true)
		//{
		//	if (card?.Data == null)
		//	{
		//		return new();
		//	}
		//	ZoneParent zoneParent = card.GetZoneParent();
		//	CardObject activeCard = card.linkedObj;

		//	while (activeCard.TryGetChildCard(out CardObject newCard))
		//	{
		//		if ((BaseGameManager.Instance.Rules.GetRankValue(activeCard.Rank) -
		//				BaseGameManager.Instance.Rules.GetRankValue(newCard.Rank)) == (ascending ? 1 : -1))
		//		{
		//			activeCard = newCard;
		//		}
		//		else
		//		{
		//			break;
		//		}
		//	}

		//	List<Card> cardChain = new();
		//	cardChain.Add(activeCard.Data);

		//	while (activeCard.TryGetParentCard(out CardObject newCard))
		//	{
		//		if (!newCard.Flipped)
		//		{
		//			break;
		//		}

		//		if ((BaseGameManager.Instance.Rules.GetRankValue(activeCard.Rank) -
		//			BaseGameManager.Instance.Rules.GetRankValue(newCard.Rank)) == (ascending ? -1 : 1))
		//		{
		//			cardChain.Add(newCard.Data);
		//			activeCard = newCard;
		//		}
		//		else
		//		{
		//			break;
		//		}
		//	}

		//	// Finally, reverse the card chain (since we want it to be from the first card in the chain down
		//	cardChain.Reverse();
		//	return cardChain;
		//}

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
			public List<SaveData> zoneData = new();
		}

		public SaveData Save()
		{
			BoardSaveData data = new();

			foreach (Card card in allCards)
			{
				data.cardData.Add(card.linkedObj.Save());
			}
			foreach (ZoneParent zone in AllZoneParents)
			{
				data.zoneData.Add(zone.Save());
			}

			return data;
		}

		public async void Load(SaveData saveData)
		{
			try
			{
				BoardSaveData data = saveData as BoardSaveData;

				// Create all cards (Setting cards based on the ID)
				GameTaskManager.Instance.AddTask(GenerateCards());

				List<CardObject.CardSaveData> cardSaveData = data.cardData.Cast<CardObject.CardSaveData>().ToList();

				if (allCards.Count > cardSaveData.Count)
				{
					// If we don't have enough cards, we are unable to load safely. If we have too many, we'll just ignore the extras
					throw new Exception("Incorrect number of saved cards");
				}

				// Clear all parent-child relationships to prevent unintended moves
				foreach (ZoneParent parent in AllZoneParents)
				{
					parent.RemoveAllCards();
				}

				//await Task.Delay(100);

				// Ensure both allCards and cardSaveData are ordered by ID
				// If the file hasn't been edited, then this should be the case regardless, but for safety we confirm the order
				cardSaveData = cardSaveData.OrderBy(o => o.cardData.ID).ToList();

				for (int i = 0; i < allCards.Count; i++)
				{
					if (allCards[i].ID != cardSaveData[i].cardData.ID)
					{
						throw new Exception($"Card ID mismatch at index {i}");
					}

					MoveCard(allCards[i], cardSaveData[i].zone, cardSaveData[i].zoneIndex,
																teleport: true, canUndo: false);

					//await Task.Delay(100);

					allCards[i].linkedObj.Load(cardSaveData[i]);
				}

				//await GameTaskManager.Instance.WhenAll();

				if (AllZoneParents.Count > data.zoneData.Count)
				{
					throw new IndexOutOfRangeException("Incorrect number of saved zones");
				}

				for (int i = 0; i < AllZoneParents.Count; i++)
				{
					// Order should be identical unless modified, if modified, so be it, but still confirm that they are the right zone
					if (AllZoneParents[i].Zone != (data.zoneData[i] as ZoneParent.ZoneSaveData).zone)
					{
						throw new Exception($"Zone type mismatch at index {i}");
					}
					AllZoneParents[i].Load(data.zoneData[i]);
				}

				foreach (ZoneParent stock in stockParents)
				{
					DeckObject deck = stock.GetComponent<DeckObject>();
					deck.Data.SyncCards();
					deck.SetVisible();
				}
			}
			catch (Exception e)
			{
				LoadFailed(e.Message);
			}
		}
		
		public void LoadFailed(string reason)
		{
			BaseGameManager.Instance.LoadFailed(reason);
		}
	}
}