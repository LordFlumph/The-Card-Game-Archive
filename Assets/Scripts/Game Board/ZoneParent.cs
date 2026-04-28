namespace CardGameArchive
{
	using System;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Linq;

	public class ZoneParent : MonoBehaviour, ISaveable
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

		[SerializeField] bool squishCards = false, squishUnflipped = true;
		[SerializeField] int cardsBeforeSquish = 10;

		[SerializeField] bool coverCards = false;
		[SerializeField] int coverLimit = 0;

		public bool UseOperations = true;

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

			int parentIndex = childCards.Count - 1;

			if (addLowerChain)
			{
				List<Card> cardChain = BaseGameManager.Instance.Rules.GetCardChain(card);

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

			if (parentIndex >= 0)
			{
				card.linkedObj.transform.SetParent(childCards[parentIndex].transform);
			}
			else
			{
				card.linkedObj.transform.SetParent(transform);
				offset.x = 0; offset.y = 0;
			}

			Task moving = card.linkedObj.MoveCard(offset, timeToMove, teleport);


			HandleCover();
			HandleSquish();

			await moving;
		}

		public void RemoveCard(Card card, bool removeLowerChain = true)
		{
			if (removeLowerChain)
			{
				List<Card> cardChain = BaseGameManager.Instance.Rules.GetCardChain(card);
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

			HandleCover();
			HandleSquish();
		}

		public void RemoveAllCards(bool nullParent = true)
		{
			if (nullParent)
			{
				foreach (CardObject card in childCards)
				{
					card.transform.SetParent(null);
				}
			}

			childCards.Clear();

		}

		public Card GetPreviousCard(Card card)
		{
			if (!childCards.Contains(card.linkedObj))
				return null;

			int index = childCards.IndexOf(card.linkedObj);
			if (index - 1 >= 0)
				return childCards[index - 1].Data;

			return null;
		}
		public Card GetNextCard(Card card)
		{
			if (!childCards.Contains(card.linkedObj))
				return null;

			int index = childCards.IndexOf(card.linkedObj);
			if (index + 1 < childCards.Count)
				return childCards[index + 1].Data;

			return null;
		}

		public bool TryGetPreviousCard(Card card, out Card previousCard)
		{
			previousCard = GetPreviousCard(card);
			return previousCard != null;
		}
		public bool TryGetNextCard(Card card, out Card nextCard)
		{
			nextCard = GetNextCard(card);
			return nextCard != null;
		}

		public void OnCardFlipped() => HandleSquish();

		void HandleCover()
		{
			if (!UseOperations)
				return;

			if (!coverCards || childCards.Count <= 0 || coverLimit < 0)
				return;

			if (childCards.Count > coverLimit)
			{
				for (int i = 0; i <= childCards.Count - coverLimit; i++)
				{
					Vector3 newOffset = new Vector3(0, 0, PositionOffset.z);

					if (Vector3.Distance(childCards[i].transform.localPosition, newOffset) > 0.01f)
						childCards[i].MoveCard(newOffset);
				}

				for (int i = childCards.Count - coverLimit + 1; i < childCards.Count; i++)
				{
					if (Vector3.Distance(childCards[i].transform.localPosition, PositionOffset) > 0.01f)
						childCards[i].MoveCard(PositionOffset);
				}
			}
			else
			{
				for (int i = 1; i < childCards.Count; i++)
				{
					if (Vector3.Distance(childCards[i].transform.localPosition, PositionOffset) > 0.01f)
						childCards[i].MoveCard(PositionOffset);
				}
			}
		}

		void HandleSquish()
		{
			if (!UseOperations)
				return;

			if (squishCards || squishUnflipped)
			{
				List<Card> lastCardChain = BaseGameManager.Instance.Rules.GetCardChain(this);
				Vector3 newOffset = PositionOffset * 0.5f;
				for (int i = 1; i < childCards.Count; i++)
				{
					if (squishCards)
					{
						Vector3 modifiedOffset = newOffset;
						modifiedOffset = Vector3.Lerp(modifiedOffset, PositionOffset, Mathf.InverseLerp(0, childCards.Count - cardsBeforeSquish, i));

						// If we should squish this card, do it
						if (i < childCards.Count - cardsBeforeSquish)
						{
							// Avoid squishing members of the last card chain
							if (!lastCardChain.Contains(childCards[i].Data))
								childCards[i].MoveCard(modifiedOffset);
						}
						else
						{
							childCards[i].MoveCard(PositionOffset);
						}
					}

					// We do this after as it should take priority at squishing cards
					if (squishUnflipped)
					{
						if (!childCards[i].Flipped)
						{
							childCards[i].MoveCard(newOffset);
						}
					}
				}
			}
		}

		public class ZoneSaveData : SaveData
		{
			public GameBoard.CardZone zone;
			public List<int> cardIDOrder = new();
		}

		public SaveData Save()
		{
			ZoneSaveData data = new();
			data.zone = Zone;
			foreach (CardObject card in childCards)
			{
				data.cardIDOrder.Add(card.ID);
			}
			return data;
		}

		public void Load(SaveData saveData)
		{
			try
			{
				if (childCards.Count == 0)
					return;

				// Remove all parents
				foreach (CardObject card in childCards)
				{
					card.transform.SetParent(null);
				}

				ZoneSaveData data = saveData as ZoneSaveData;
				childCards = childCards.OrderBy(o => data.cardIDOrder.IndexOf(o.ID)).ToList();

				childCards[0].transform.SetParent(transform);
				childCards[0].MoveCard(new Vector3(0, 0, PositionOffset.z), teleport: true);
				for (int i = 1; i < childCards.Count; i++)
				{
					childCards[i].transform.SetParent(childCards[i - 1].transform);
					childCards[i].MoveCard(PositionOffset, teleport: true);
				}

				HandleCover();
				HandleSquish();
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
