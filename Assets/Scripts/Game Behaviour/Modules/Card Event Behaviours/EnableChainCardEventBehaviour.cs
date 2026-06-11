namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "EnableChainCardEventBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Card Event Behaviours/Enable Chain On Move")]
	public class EnableChainCardEventBehaviour : BaseCardEventBehaviour
    {
		protected override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from != null && !IsFromBlacklisted(eventData))
				EnableLastChain(eventData.from);
		}
		protected override void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.to != null && !IsToBlacklisted(eventData))
				EnableLastChain(eventData.to);
		}

		void EnableLastChain(ZoneParent parent)
		{
			foreach (CardObject card in parent.Cards)
			{
				card.Data.SetInteractable(false);
			}

			List<Card> cardChain = BaseGameRules.ActiveRules.GetCardChain(parent);
			foreach (Card card in cardChain)
			{
				card.SetInteractable(true);
			}
		}
	}

}