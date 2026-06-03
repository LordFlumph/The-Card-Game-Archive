namespace CardGameArchive.Behaviours
{
	using CardGameArchive.TMP;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class DealToWasteLoopDeckBehaviour : BaseDeckBehaviour
	{
		[SerializeField] int cardsToDeal = 1;
		[Tooltip("If true, the deck will deal all of its cards before recycling, regardless of if the cards dealt match with cardsToDeal")]
		[SerializeField] bool dealAllCards = true;

		[SerializeField] int dealDelay = 0;

		public override async void OnDeckTapped(Deck deck)
		{
			ZoneParent waste = GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Waste)[0];
			ZoneParent stock = deck.linkedObj.GetComponent<ZoneParent>();

			if (deck.RemainingCards == 0 || (!dealAllCards && deck.RemainingCards < cardsToDeal))
			{
				Recycle(stock, waste, deck);
			}
			else
			{
				int dealt = 0;
				while (dealt < cardsToDeal && deck.RemainingCards > 0)
				{
					dealt++;
					Card card = deck.Draw();

					bool lastCard = dealt >= cardsToDeal || deck.RemainingCards == 0;
					GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, waste, forceContingent: lastCard));

					if (dealDelay > 0)
						await Awaitable.WaitForSecondsAsync(dealDelay);
				}
				
			}
		}

		void Recycle(ZoneParent stock, ZoneParent waste, Deck deck)
		{
			List<CardObject> cards = waste.Cards;

			cards.Reverse();

			CardObject firstCard = cards[0];
			foreach (CardObject card in cards)
			{
				deck.AddCard(card.Data);
				card.Data.SetInteractable(false);
				GameTaskManager.Instance.AddTask(card.Data.SetFlipped(false));
				StandardGameManager.Instance.MoveTaken(new(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(card.Data, false, true)));

				bool lastCard = cards[^1] == card;
				GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card.Data, GameBoard.CardZone.Stock, forceContingent: !lastCard));
			}
		}
	}
}
