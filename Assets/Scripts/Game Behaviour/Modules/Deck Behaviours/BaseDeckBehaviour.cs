namespace CardGameArchive.Behaviours
{
    using System.Collections.Generic;
	using UnityEngine;
	public abstract class BaseDeckBehaviour : BaseBehaviour
	{
		[SerializeField] List<BaseBehaviourBlocker> blockingConditions;
		public void DeckTapped(Deck deck)
		{
			foreach (BaseBehaviourBlocker blocker in blockingConditions)
			{
				if (blocker.BlockBehaviour())
					return;
			}

			if (ModuleEventManager.Instance != null)
				ModuleEventManager.Instance.OnDeckTapped.Invoke(deck);

			OnDeckTapped(deck);
		}
		protected abstract void OnDeckTapped(Deck deck);
	}
}