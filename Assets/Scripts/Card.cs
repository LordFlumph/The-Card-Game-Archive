namespace CardGameArchive
{
    using UnityEngine;

    [System.Serializable]
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

        [field: SerializeField] public CardValue Value { get; private set; }
        [field: SerializeField] public CardSuit Suit { get; private set; }

        public GameObject linkedUIObj;

        public Card(CardValue value, CardSuit suit, GameObject obj = null)
        {
            Value = value;
            Suit = suit;
            linkedUIObj = obj;
        }
    }
}
