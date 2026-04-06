namespace CardGameArchive
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	[Serializable]
	public class Card
	{
		public enum CardRank
		{
			Ace,
			Two,
			Three,
			Four,
			Five,
			Six,
			Seven,
			Eight,
			Nine,
			Ten,
			Jack,
			Queen,
			King,
			Joker
		}

		public enum CardSuit
		{
			Clubs,
			Diamonds,
			Hearts,
			Spades
		}

		public enum CardColour
		{
			Red,
			Black
		}
		public static Dictionary<CardSuit, CardColour> SuitColours { get; } = new()
		{
			{ CardSuit.Clubs, CardColour.Black },
			{ CardSuit.Diamonds, CardColour.Red },
			{ CardSuit.Hearts, CardColour.Red },
			{ CardSuit.Spades, CardColour.Black }
		};


		[Serializable]
		public struct CardData
		{
			public CardSuit suit;
			public CardRank rank;
			public CardData(CardRank rank, CardSuit suit)
			{
				this.rank = rank;
				this.suit = suit;
			}
		}

		[field: SerializeField] public CardData Data { get; private set; }
		public CardSuit Suit => Data.suit;
		public CardRank Rank => Data.rank;

		public CardObject linkedObj;

		public bool Flipped { get; private set; } = false;

		public bool Interactable { get; private set; } = true;

		public Card(CardRank value, CardSuit suit, CardObject obj = null)
		{
			Data = new(value, suit);
			linkedObj = obj;
		}

		public async Task SetFlipped(bool flipped, bool clockwise = false, bool instant = false)
		{
			if (Flipped == flipped)
				return;

			Flipped = flipped;

			if (!instant)
			{
				if (!clockwise)
				{
					while (linkedObj.spriteRenderer.transform.localRotation.eulerAngles.y < 90)
					{
						linkedObj.spriteRenderer.transform.localRotation *= Quaternion.Euler(0, 720 * Time.deltaTime, 0);
						await Task.Yield();
					}
				}
				else
				{
					do
					{
						linkedObj.spriteRenderer.transform.localRotation *= Quaternion.Euler(0, -720 * Time.deltaTime, 0);
						await Task.Yield();
					} while (linkedObj.spriteRenderer.transform.localRotation.eulerAngles.y > 270);
				}				
			}

			if (!flipped)
			{
				linkedObj.spriteRenderer.sprite = CardSpriteCollection.Instance.GetCardBack();
			}
			else
			{
				linkedObj.spriteRenderer.sprite = CardSpriteCollection.Instance[Data];
			}

			if (!instant)
			{
				if (!clockwise)
				{
					linkedObj.spriteRenderer.transform.localRotation = Quaternion.Euler(0, 270, 0);
					while (linkedObj.spriteRenderer.transform.localRotation.eulerAngles.y < 359.9 && linkedObj.spriteRenderer.transform.localRotation.eulerAngles.y > 80)
					{
						linkedObj.spriteRenderer.transform.localRotation *= Quaternion.Euler(0, 720 * Time.deltaTime, 0);
						await Task.Yield();
					}
				}
				else
				{
					linkedObj.spriteRenderer.transform.localRotation = Quaternion.Euler(0, 90, 0);
					while (linkedObj.spriteRenderer.transform.localRotation.eulerAngles.y < 300)
					{
						linkedObj.spriteRenderer.transform.localRotation *= Quaternion.Euler(0, -720 * Time.deltaTime, 0);
						await Task.Yield();
					}
				}
				

				linkedObj.spriteRenderer.transform.localRotation = Quaternion.identity;
			}
		}
	
		public void SetInteractable(bool interactable)
		{
			this.Interactable = interactable;
			if (linkedObj != null)
				linkedObj.collider.enabled = interactable;
		}

		public ZoneParent GetZoneParent()
		{
			if (linkedObj != null)
				return linkedObj.GetZoneParent();
			return null;
		}

		public override string ToString()
		{
			return $"{Rank}, {Suit}";
		}
	}
}
