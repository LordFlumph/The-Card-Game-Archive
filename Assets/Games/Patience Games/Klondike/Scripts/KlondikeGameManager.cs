namespace CardGameArchive.Solitaire.Klondike
{
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Linq;
	using static Deck;
	using UnityEngine.Rendering;

	public class KlondikeGameManager : BaseGameManager
	{
		public override void SetRules()
		{
			Rules = new KlondikeGameRules();
		}

		public override async void StartGame()
		{
			gameBoard.GetZoneParents(GameBoard.CardZone.Stock)[0].GetComponent<DeckObject>().InitializeDeck(Deck);

			Deck.Initialise(DeckType.Full52, gameBoard.GetZoneParents(GameBoard.CardZone.Stock)[0].GetComponent<DeckObject>());
			Deck.Shuffle();

			List<Task> dealingTasks = new();

			for (int i = 0; i < 7; i++)
			{
				for (int j = i; j < 7; j++)
				{
					Card card = Deck.Draw();
					dealingTasks.Add(gameBoard.MoveCard
						(
							card: card,
							destination: GameBoard.CardZone.Tableau,
							index: j,
							fromStock: true
						));

					// Last card in column
					if (j == i)
					{
						card.SetFlipped(true);
					}

					await Task.Delay(50);
				}
			}

			gameBoard.OnCardMoveStart += OnCardMoveStart;
		}

		public override void OnCardTapped(Card card)
		{
			AutoMove(card);
		}
		public override void OnCardGrabbed(Card card)
		{
			// Move card above everything else
			card.linkedObj.transform.position = new(card.linkedObj.transform.position.x, card.linkedObj.transform.position.y, -1);
		}
		public override void OnCardDropped(Card card)
		{
			// Decide if the card can be placed here, if not, move it back
		}

		public override async void OnDeckTapped(Deck deck)
		{
			ZoneParent wasteZone = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0];

			wasteZone.SetOperations(false);

			List<Task> tasks = new();

			if (deck.RemainingCards > 0)
			{
				for (int i = 0; i < 3; i++)
				{
					Card card = Deck.Draw();

					// This means we've reached the end of the deck
					if (card == null)
						break;

					tasks.Add(GameBoard.Instance.MoveCard(card, GameBoard.CardZone.Waste,
													fromStock: true));

					card.SetFlipped(true);
				}
			}

			// Return all cards in waste into deck
			else
			{
				Transform waste = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0].transform;
				List<CardObject> cards = waste.GetAllChildren().Select(o => o.GetComponent<CardObject>()).Where(o => o != null).ToList();

				cards.Reverse();
				foreach (CardObject card in cards)
				{
					deck.AddCard(card.CardData);
					card.CardData.SetFlipped(false);
					tasks.Add(GameBoard.Instance.MoveCard(card.CardData, GameBoard.CardZone.Stock));
				}
			}

			await Task.WhenAll(tasks);

			wasteZone.SetOperations(true);
		}

		public override List<ZoneParent> GetPossibleMoves(Card card)
		{
			if (!Rules.CanCardMove(card))
				return new();

			List<ZoneParent> possibleParents = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation);
			possibleParents.AddRange(GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Tableau));

			for (int i = 0; i < possibleParents.Count; i++)
			{
				if (!Rules.IsMoveValid(card, possibleParents[i]))
				{
					possibleParents.RemoveAt(i);
					i--;
				}
			}

			return possibleParents;
		}

		public override async Task AutoMoveCards()
		{
			List<Card> possibleCards = new();

			possibleCards.Add(gameBoard.GetZoneParents(GameBoard.CardZone.Waste)[0].BottomCard);
		}

		/// <summary>
		/// Automatically move this card somewhere it will 
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		public override async Task AutoMove(Card card)
		{
			List<ZoneParent> possibleParents = GetPossibleMoves(card);

			if (possibleParents.Count <= 0)
				return;

			// If we can move to the foundation, do so
			if (possibleParents[0].Zone == GameBoard.CardZone.Foundation)
			{
				GameBoard.Instance.MoveCard(card, possibleParents[0]);
				return;
			}

			// Work out which Tableau column is best to move to (has the longest card chain)
			int highestCardChain = 0;
			ZoneParent highestParent = possibleParents[0];
			foreach (ZoneParent parent in possibleParents)
			{
				List<Card> cardChain = GameBoard.Instance.GetCardChain(parent);
				if (cardChain.Count > highestCardChain)
				{
					highestCardChain = cardChain.Count;
					highestParent = parent;
				}
			}

			GameBoard.Instance.MoveCard(card, highestParent);
		}		
	
		protected override async void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from != null)
			{
				if (eventData.from.BottomCard != null)
				{
					eventData.from.BottomCard.SetFlipped(true);
				}
			}
		}

		private void OnDisable()
		{
			if (gameBoard != null)
				gameBoard.OnCardMoveStart -= OnCardMoveStart;
		}
	}
}
