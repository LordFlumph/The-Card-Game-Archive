namespace CardGameArchive.Behaviours
{
	
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "DealToWasteLoopDeckBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Deck Behaviours/Deal To Waste Loop")]
	public class DealToWasteLoopDeckBehaviour : BaseDeckBehaviour
	{
		[SerializeField] int cardsToDeal = 1;
		[Tooltip("If true, the deck will deal all of its cards before recycling, regardless of if the cards dealt match with cardsToDeal")]
		[SerializeField] bool dealAllCards = true;

		[SerializeField] float dealDelay = 0;

		[Tooltip("If set to a value greater than 0, the deck will only recycle a certain number of times before stopping. Set to -1 for infinite loops")]
		[SerializeField] int maxLoops = -1;

		protected override async void OnDeckTapped(Deck deck)
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

					GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card, waste, forceContingent: dealt != 1));

					if (dealDelay > 0)
						await Awaitable.WaitForSecondsAsync(dealDelay);
				}				
			}
		}

		void Recycle(ZoneParent stock, ZoneParent waste, Deck deck)
		{
			if (maxLoops != -1)
			{
				ValueHolderRuntimeData runtimeData = StandardGameManager.Instance.GetRuntimeData<ValueHolderRuntimeData>(o => o.identifier == ValueHolderRuntimeData.Identifier.DealToWasteLoopValue);
				if (runtimeData == null)
				{
					throw new System.Exception("No runtime data found for DealToWasteLoopValue behaviour");
				}
				if (runtimeData.GetValue<int>() >= maxLoops)
					return;

				runtimeData.SetValue(runtimeData.GetValue<int>() + 1);
			}

			List<CardObject> cards = waste.Cards;

			cards.Reverse();

			CardObject firstCard = cards[0];
			foreach (CardObject card in cards)
			{
				deck.AddCard(card.Data);
				card.Data.SetInteractable(false);
				GameTaskManager.Instance.AddTask(card.Data.SetFlipped(false));
				StandardGameManager.Instance.MoveTaken(new(GameMove.MoveType.CardFlipped, new GameMove.CardFlippedData(card.Data, false, firstCard != card)));

				GameTaskManager.Instance.AddTask(GameBoard.Instance.MoveCard(card.Data, GameBoard.CardZone.Stock, forceContingent: true));
			}
		}
	}
}
