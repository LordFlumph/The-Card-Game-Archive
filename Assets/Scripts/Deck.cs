namespace CardGameArchive
{
	using System.Collections.Generic;
	using System;
	using static Card;

	[Serializable]
	public class Deck
	{
		private List<Card> cardList = new();
		public int RemainingCards => cardList.Count;

		DeckObject deckObj;

		public enum DeckType
		{
			Full52,
			Full54,
			Full108,
			OneSuit52,
			OneSuit54,
			TwoSuit52,
			TwoSuit54
		}

		public void Initialise(DeckType deckType, DeckObject deckObj)
		{
			this.deckObj = deckObj;

			cardList.Clear();

			switch (deckType)
			{
				case DeckType.Full52:
					foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
					{
						foreach (CardRank value in Enum.GetValues(typeof(CardRank)))
						{
							if (value != CardRank.Joker)
								cardList.Add(new Card(value, suit));
						}
					}
					break;

				case DeckType.Full54:
					foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
					{
						foreach (CardRank value in Enum.GetValues(typeof(CardRank)))
						{
							if (value != CardRank.Joker)
								cardList.Add(new Card(value, suit));
						}
					}
					cardList.Add(new(CardRank.Joker, CardSuit.Clubs));
					cardList.Add(new(CardRank.Joker, CardSuit.Diamonds));
					break;

				case DeckType.Full108:

					foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
					{
						foreach (CardRank value in Enum.GetValues(typeof(CardRank)))
						{
							if (value != CardRank.Joker)
							{
								cardList.Add(new Card(value, suit));
								cardList.Add(new Card(value, suit));
							}

						}
					}
					cardList.Add(new(CardRank.Joker, CardSuit.Clubs));
					cardList.Add(new(CardRank.Joker, CardSuit.Diamonds));
					cardList.Add(new(CardRank.Joker, CardSuit.Hearts));
					cardList.Add(new(CardRank.Joker, CardSuit.Spades));
					break;

				case DeckType.OneSuit52:
					for (int i = 0; i < 4; i++)
					{
						foreach (CardRank value in Enum.GetValues(typeof(CardRank)))
						{
							if (value != CardRank.Joker)
								cardList.Add(new Card(value, CardSuit.Clubs));
						}
					}
					break;

				case DeckType.OneSuit54:
					for (int i = 0; i < 4; i++)
					{
						foreach (CardRank value in Enum.GetValues(typeof(CardRank)))
						{
							cardList.Add(new Card(value, CardSuit.Clubs));
						}
					}
					break;

				case DeckType.TwoSuit52:
					for (int i = 0; i < 4; i++)
					{
						foreach (CardRank value in Enum.GetValues(typeof(CardRank)))
						{
							if (value != CardRank.Joker)
							{
								cardList.Add(new Card(value, CardSuit.Clubs));
								cardList.Add(new(value, CardSuit.Hearts));
							}

						}
					}
					break;

				case DeckType.TwoSuit54:
					for (int i = 0; i < 4; i++)
					{
						foreach (CardRank value in Enum.GetValues(typeof(CardRank)))
						{
							cardList.Add(new Card(value, CardSuit.Clubs));
							cardList.Add(new(value, CardSuit.Hearts));
						}
					}
					break;
			}

			Shuffle();
		}

		public void Shuffle()
		{
			cardList.Shuffle();
		}

		public Card Draw()
		{
			if (cardList.Count == 0)
				return null;

			Card card = cardList[^1];
			cardList.RemoveAt(cardList.Count-1);

			deckObj.SetVisible();

			return card;
		}
		public void AddCard(Card card)
		{
			if (card != null && !cardList.Contains(card))
				cardList.Add(card);

			deckObj.SetVisible();
		}
	}
}