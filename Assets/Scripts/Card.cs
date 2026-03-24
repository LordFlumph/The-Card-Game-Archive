namespace CardGameArchive
{
	using UnityEngine;
    using System;

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

        [Serializable]
        public struct CardData
        {
            public CardSuit suit;
            public CardRank rank;
            public CardData(CardRank value, CardSuit suit)
            {
                this.rank = value;
                this.suit = suit;
			}

			//public static bool operator ==(CardData a, CardData b) => a.value == b.value && a.suit == b.suit;
			//public static bool operator !=(CardData a, CardData b) => a.value != b.value && a.suit != b.suit;
		}
        [field: SerializeField] public CardData Data { get; private set; }

		public CardObject linkedObj;

        public bool interactable = false;

        public Card(CardRank value, CardSuit suit, CardObject obj = null)
        {
            Data = new(value, suit);
            linkedObj = obj;
        }

        public void SetFlipped(bool flipped)
        {
			linkedObj.SetFlipped(flipped);
		}
    }
}
