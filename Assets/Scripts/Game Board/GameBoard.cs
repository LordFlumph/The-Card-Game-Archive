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

			public CardMoveEvent(Card card, ZoneParent from, ZoneParent to)
			{
				this.card = card;
				this.from = from;
				this.to = to;
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

		public async void MoveCard(Card card, ZoneParent destination,
									bool fromStock = false, int stockIndex = 0,
									bool teleport = false,
									float timeToMove = -1)
		{
			switch (destination.Zone)
			{
				case CardZone.Stock:
					await MoveCard(card, CardZone.Stock, stockParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove);
					break;
				case CardZone.Waste:
					await MoveCard(card, CardZone.Waste, wasteParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove);
					break;
				case CardZone.Foundation:
					await MoveCard(card, CardZone.Foundation, foundationParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove);
					break;
				case CardZone.Tableau:
					await MoveCard(card, CardZone.Tableau, tableauParents.IndexOf(destination), fromStock, stockIndex, teleport, timeToMove);
					break;
			}
		}

		public async Task MoveCard(Card card, CardZone destination,
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

			ZoneParent originalZone = card.linkedObj.GetZoneParent();

			if (originalZone != null)
			{
				originalZone.RemoveCard(card);
			}

			CardMoveEvent moveEventData = new(card, originalZone, targetZone);
			OnCardMoveStart?.Invoke(moveEventData);

			await targetZone.PlaceCard(card, timeToMove, teleport);

			card.SetInteractable(setInteractable);

			OnCardMoveFinish?.Invoke(moveEventData);
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
			if (card?.CardData == null)
			{
				return new();
			}

			CardObject activeCard = card;
			while (activeCard.transform.childCount > 0)
			{
				if (activeCard.transform.GetChild(0).TryGetComponent(out CardObject newCard))
				{
					if ((BaseGameManager.Instance.Rules.GetRankValue(activeCard.CardData.Rank) -
						BaseGameManager.Instance.Rules.GetRankValue(newCard.CardData.Rank)) == (ascending ? 1 : -1))
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
			cardChain.Add(activeCard.CardData);

			while (activeCard.transform.parent != null)
			{
				if (activeCard.transform.parent.TryGetComponent(out CardObject newCard))
				{
					if (!newCard.CardData.Flipped)
					{
						break;
					}

					if ((BaseGameManager.Instance.Rules.GetRankValue(activeCard.CardData.Rank) -
						BaseGameManager.Instance.Rules.GetRankValue(newCard.CardData.Rank)) == (ascending ? -1 : 1))
					{
						cardChain.Add(newCard.CardData);
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