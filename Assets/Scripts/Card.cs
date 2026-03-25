namespace CardGameArchive
{
	using NUnit.Framework;
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using static CardGameArchive.Card;

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

		public bool interactable = false;

		public Card(CardRank value, CardSuit suit, CardObject obj = null)
		{
			Data = new(value, suit);
			linkedObj = obj;
		}

		public void SetFlipped(bool flipped)
		{
			if (!flipped)
			{
				linkedObj.spriteRenderer.sprite = CardSpriteCollection.Instance.GetCardBack();
			}
			else
			{

				linkedObj.spriteRenderer.sprite = CardSpriteCollection.Instance[Data];
			}
		}
	}
}
