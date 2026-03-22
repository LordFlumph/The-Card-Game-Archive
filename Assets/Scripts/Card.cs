namespace CardGameArchive
{
	using UnityEngine;
    using System;

    [Serializable]
    public class Card
    {
        public enum CardValue
        {
            Ace,
            One,
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
            public CardValue value;
            public CardData(CardValue value, CardSuit suit)
            {
                this.value = value;
                this.suit = suit;
			}

			//public static bool operator ==(CardData a, CardData b) => a.value == b.value && a.suit == b.suit;
			//public static bool operator !=(CardData a, CardData b) => a.value != b.value && a.suit != b.suit;
		}
        [field: SerializeField] public CardData Data { get; private set; }

		public GameObject linkedObj;

        public bool interactable = false;

        public Card(CardValue value, CardSuit suit, GameObject obj = null)
        {
            Data = new(value, suit);
            linkedObj = obj;
        }

        public void SetVisible(bool visibile)
        {
            if (visibile)
            {

            }
            else
            {

            }
        }
    }
}
