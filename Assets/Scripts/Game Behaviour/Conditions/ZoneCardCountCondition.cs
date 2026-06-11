namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "ZoneCardCountCondition", menuName = "Card Game Archive/Game Behaviour/Conditions/Zone Card Count")]
	public class ZoneCardCountCondition : BaseCondition<ZoneParent>
	{
		[SerializeField] int targetCardCount;
		[Tooltip("If true, the condition will only be met if all zones have exactly the target card count. If false, it is greater than or equal to")]
		[SerializeField] bool exactOnly = false;
		public override bool ConditionMet(ZoneParent parent)
		{
			if (exactOnly)
			{
				if (parent.CardCount != targetCardCount)
					return false;
			}
			else
			{
				if (parent.CardCount < targetCardCount)
					return false;
			}
			return true;
		}
	}

}