namespace CardGameArchive.Solitaire.Spiderette
{
	using CardGameArchive.Solitaire.Spider;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using UnityEngine;

	public class SpideretteGameManager : SpiderGameManager
	{
		protected override void SetGame()
		{
			Rules = new SpideretteGameRules();
		}

        protected override async Task StartGame()
		{
			int verificationCounter = 1;
			Deck deck = gameBoard.GetDeck();
			while (verificationCounter < 100)
			{
				deck.Shuffle(false);
				if (VerifyDeck())
				{
					break;
				}
				verificationCounter++;
			}

			Debug.Log("Verification attempts: " + verificationCounter);
			if (verificationCounter >= 100)
			{
				Debug.LogWarning("Failed to verify deck");
			}

			GameTaskManager.Instance.AddTask(gameBoard.GenerateCards());
			GameTaskManager.Instance.QueueTask(() => Awaitable.WaitForSecondsAsync(0.5f));

			await GameTaskManager.Instance.WhenAll();

			for (int i = 0; i < 7; i++)
			{
				for (int j = i; j < 7; j++)
				{
					Card card = deck.Draw();
					GameTaskManager.Instance.AddTask(gameBoard.MoveCard
						(
							card: card,
							destination: GameBoard.CardZone.Tableau,
							index: j,
							timeToMove: 0.15f,
							canUndo: false,
							affectCardChain: false
						));

					// Last card in column
					if (j == i)
					{
						card.SetInteractable(true);
						GameTaskManager.Instance.AddTask(card.SetFlipped(true));
					}

					await Awaitable.WaitForSecondsAsync(0.05f);
				}
			}

			await GameTaskManager.Instance.WhenAll();
		}

		protected override bool VerifyDeck()
		{
			return true;
			Deck deck = gameBoard.GetDeck();
			List<List<Card>> deals = new();
			deals.Add(new(deck.Cards.GetRange(0, 10)));
			deals.Add(new(deck.Cards.GetRange(10, 10)));
			deals.Add(new(deck.Cards.GetRange(20, 10)));
			deals.Add(new(deck.Cards.GetRange(30, 10)));
			deals.Add(new(deck.Cards.GetRange(40, 10)));
			deals.Add(new(deck.Cards.GetRange(50, 10)));
			deals.Add(new(deck.Cards.GetRange(60, 10)));
			deals.Add(new(deck.Cards.GetRange(70, 10)));
			deals.Add(new(deck.Cards.GetRange(80, 10)));
			deals.Add(new(deck.Cards.GetRange(90, 10)));
			List<Card> visibleCards = deck.Cards.GetRange(deck.Cards.Count-54, 10);

			// Confirm there are at least 2 useful moves (same suit)
			int usefulMoves = 0;
			foreach (Card card in visibleCards)
			{
				foreach (Card targetCard in visibleCards)
				{
					if (card == targetCard)
						continue;

					if (card.Suit == targetCard.Suit)
					{
						if (Rules.GetRankValue(targetCard) - Rules.GetRankValue(card) == 1)
						{
							usefulMoves++;
						}
					}
				}
			}

			if (usefulMoves < 3)
				return false;


			// Confirm that there are no more than 3 of the same Rank present			
			if (visibleCards.GroupBy(o => o.Rank).Any(o => o.Count() > 3))
				return false;


			// Confirm that there is between 4 and 7 of the same suit are present in visible cards
			if (Name != GameTerms.GameName.SpiderOneSuit)
			{
				if (!(visibleCards.GroupBy(o => o.Suit).Any(o => o.Count() is >= 4 and <= 7)))
					return false;
			}

			// Confirm that there is no huge gap between card ranks
			visibleCards = visibleCards.OrderBy(o => Rules.GetRankValue(o)).ToList();
			for (int i = 1; i < visibleCards.Count; i++)
			{
				if (Rules.GetRankValue(visibleCards[i]) - Rules.GetRankValue(visibleCards[i-1]) > 6)
				{
					return false;
				}
			}


			// Confirm that every deal has no more than 2 of the same rank
			foreach (var deal in deals)
			{
				if (deal.GroupBy(o => o.Rank).Any(o => o.Count() > 2))
					return false;
			}


			// Lastly, confirm that every deal has at least 1 good move, and 2 possible moves

			usefulMoves = 0;
			int possibleMoves = 0;
			foreach (var deal in deals)
			{
				usefulMoves = 0;
				possibleMoves = 0;
				foreach (Card card in deal)
				{
					foreach (Card targetCard in deal)
					{
						if (card == targetCard)
							continue;

						if (Rules.GetRankValue(targetCard) - Rules.GetRankValue(card) == 1)
						{
							possibleMoves++;
							if (card.Suit == targetCard.Suit)
							{
								usefulMoves++;
							}

							if (possibleMoves >= 2 && usefulMoves >= 1)
								break;							
						}
						
					}
					if (possibleMoves >= 2 && usefulMoves >= 1)
						break;
				}

				if (possibleMoves < 2 || usefulMoves < 1)
					return false;
			}

			return true;
		}
				
	}
}