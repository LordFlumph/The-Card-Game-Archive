namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;
	public abstract class BaseDeckBehaviour : ScriptableObject
	{
		[SerializeField] List<BaseBehaviourBlocker> blockingConditions;
		public void DeckTapped(Deck deck)
		{
			foreach (BaseBehaviourBlocker blocker in blockingConditions)
			{
				if (blocker.BlockBehaviour())
					return;
			}

			OnDeckTapped(deck);
		}
		protected abstract void OnDeckTapped(Deck deck);
	}
}