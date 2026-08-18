namespace CardGameArchive.Behaviours
{
    using UnityEngine;

	[CreateAssetMenu(fileName = "CardCanMoveCondition", menuName = "Card Game Archive/Game Behaviour/Conditions/Card Can Move")]
	public class CardCanMoveCondition : BaseCondition<Card>
	{
		public override bool ConditionMet(Card context)
		{
			return BaseGameRules.ActiveRules.CanCardMove(context);
		}
	}
}
