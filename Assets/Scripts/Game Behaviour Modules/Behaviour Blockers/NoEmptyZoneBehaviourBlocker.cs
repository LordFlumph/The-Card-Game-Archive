namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "NoEmptyZoneBehaviourBlocker", menuName = "Card Game Archive/Game Behaviour/Behaviour Blockers/No Empty Zone")]
	public class NoEmptyZoneBehaviourBlocker : BaseBehaviourBlocker
	{
		[SerializeField] List<GameBoard.CardZone> zonesToCheck;
		public override bool BlockBehaviour()
		{
			List<ZoneParent> blockedParents = new();
			foreach (ZoneParent parent in GameBoard.Instance.AllZoneParents)
			{
				if (zonesToCheck.Contains(parent.Zone))
				{
					if (parent.CardCount == 0)
						blockedParents.Add(parent);
				}
			}

			if (blockedParents.Count > 0)
			{
				OnBehaviourBlocked(blockedParents);
				return true;
			}

			return false;
		}
		protected override void OnBehaviourBlocked<T>(T context)
		{
			if (context is List<ZoneParent> blockedParents)
			{
				foreach (ZoneParent parent in blockedParents)
				{
					FeedbackManager.Instance.PulseHighlight(parent.gameObject, FeedbackManager.Instance.InvalidColour);
				}
			}
			else
			{
				Debug.LogError($"Context passed to {nameof(OnBehaviourBlocked)} was not of type {typeof(List<ZoneParent>)}");
				return;
			}
		}
	}

}