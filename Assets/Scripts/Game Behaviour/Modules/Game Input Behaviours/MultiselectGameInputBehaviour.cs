namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "MultiselectGameInputBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Game Input Behaviours/Multiselect")]
	public class MultiselectGameInputBehaviour : BaseGameInputBehaviour
	{
		protected override void OnCardTapped(Card card)
		{
			if (!card.Interactable)
				return;

			CardSelectionRuntimeData selectionHolder = StandardGameManager.Instance.GetRuntimeData<CardSelectionRuntimeData>();

			if (selectionHolder.IsCardSelected(card))
			{
				selectionHolder.DeselectCard(card);
			}
			else
			{
				selectionHolder.SelectCard(card);
			}

			BaseMultiselectMoveBehaviour moveBehaviour = StandardGameManager.Instance.GetMoveBehaviour<BaseMultiselectMoveBehaviour>();
			
			if (moveBehaviour == null)
				throw new System.InvalidCastException("MoveBehaviour is not of type BaseMultiselectMoveBehaviour. Please ensure the correct MoveBehaviour is being used.");

			moveBehaviour.SelectionChanged();
		}
		protected override void OnCardDropped(Card card)
		{
			Debug.Log("Card dropped while using MultiselectGameInput. No behaviour implemented");
		}
	}

}