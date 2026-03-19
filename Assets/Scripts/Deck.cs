namespace CardGameArchive
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using static Card;

    public class Deck : MonoBehaviour
    {
        private List<Card> cardList = new();

        public enum DeckType
        {
            Full52,
            Full54,
            Full108

        }
        [field: SerializeField] private DeckType deckType { get; } = DeckType.Full52;

        private void Start()
        {
            switch (deckType)
            {
                case DeckType.Full52:
                    foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                    {
                        foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                        {
                            if (value != CardValue.Joker)
                                cardList.Add(new Card(value, suit));
                        }
                    }
                    break;

                case DeckType.Full54:
                    foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                    {
                        foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                        {
                            if (value != CardValue.Joker)
                                cardList.Add(new Card(value, suit));
                        }
                    }
                    cardList.Add(new(CardValue.Joker, CardSuit.Clubs));
                    cardList.Add(new(CardValue.Joker, CardSuit.Diamonds));
                    break;

                case DeckType.Full108:

                    foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                    {
                        foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                        {
                            if (value != CardValue.Joker)
                            {
                                cardList.Add(new Card(value, suit));
                                cardList.Add(new Card(value, suit));
                            }

                        }
                    }
                    cardList.Add(new(CardValue.Joker, CardSuit.Clubs));
                    cardList.Add(new(CardValue.Joker, CardSuit.Diamonds));
                    cardList.Add(new(CardValue.Joker, CardSuit.Hearts));
                    cardList.Add(new(CardValue.Joker, CardSuit.Spades));
                    break;
            }

            Shuffle();
        }

        public void Shuffle()
        {
            cardList.Shuffle();
        }

        public void Draw(int number = 1)
        {
            if (number <= 0)
                return;

            for (; number > 0 && cardList.Count > 0; number--)
            {
                // Tell UI to reveal a card
                // Tell GameRules we drew a card
                Card card = cardList[0];
            }
        }
    }
}