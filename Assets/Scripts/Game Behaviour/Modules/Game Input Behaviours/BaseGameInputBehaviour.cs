namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;
	public abstract class BaseGameInputBehaviour : ScriptableObject
	{
		[SerializeField] List<BaseBehaviourBlocker> blockingConditions;
		public void CardTapped(Card card)
		{
			foreach (BaseBehaviourBlocker blocker in blockingConditions)
			{
				if (blocker.BlockBehaviour())
					return;
			}
			OnCardTapped(card);
		}
		protected abstract void OnCardTapped(Card card);
		public void CardGrabbed(Card card)
		{
			foreach (BaseBehaviourBlocker blocker in blockingConditions)
			{
				if (blocker.BlockBehaviour())
					return;
			}
			OnCardGrabbed(card);
		}
		protected virtual void OnCardGrabbed(Card card) { }

		public void CardDropped(Card card)
		{
			foreach (BaseBehaviourBlocker blocker in blockingConditions)
			{
				if (blocker.BlockBehaviour())
					return;
			}
			OnCardDropped(card);
		}
		protected abstract void OnCardDropped(Card card);
	}
}