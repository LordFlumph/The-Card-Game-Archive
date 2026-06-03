namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "EnableChainCardEventBehaviour", menuName = "Game Behaviour/Card Event Behaviours/Enable Chain On Move")]
	public class EnableChainCardEventBehaviour : BaseCardEventBehaviour
    {
		public override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.from != null)
				EnableLastChain(eventData.from);
		}
		public override void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			if (eventData.to != null)
				EnableLastChain(eventData.to);
		}

		void EnableLastChain(ZoneParent parent)
		{
			foreach (CardObject card in parent.Cards)
			{
				card.Data.SetInteractable(false, false);
			}

			foreach (Card card in BaseGameManager.Instance.Rules.GetCardChain(parent))
			{
				card.SetInteractable(true);
			}
		}
	}

}