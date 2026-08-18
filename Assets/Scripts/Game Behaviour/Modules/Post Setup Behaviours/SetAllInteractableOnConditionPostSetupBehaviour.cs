namespace CardGameArchive.Behaviours
{
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "SetAllInteractableOnConditionPostSetupBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Post Setup Behaviours/Set All Interactable On Condition")]
	public class SetAllInteractableOnConditionPostSetupBehaviour : BasePostSetupBehaviour
	{
		[SerializeField] bool invertCondition = false;

		[SerializeField] BaseCondition<Card> condition;
		public override async Task FinaliseBoard()
		{
			foreach (Card card in GameBoard.Instance.AllCards)
			{
				card.SetInteractable(condition.ConditionMet(card));
				if (invertCondition)
					card.SetInteractable(!card.Interactable);
			}
		}
	}

}