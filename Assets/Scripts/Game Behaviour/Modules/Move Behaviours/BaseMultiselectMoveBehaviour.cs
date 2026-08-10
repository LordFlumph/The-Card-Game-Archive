namespace CardGameArchive.Behaviours
{
	using System.Threading.Tasks;
	using UnityEngine;

	public abstract class BaseMultiselectMoveBehaviour : BaseMoveBehaviour
	{
		public virtual void SelectionChanged()
		{
			OnSelect(StandardGameManager.Instance.GetRuntimeData<CardSelectionRuntimeData>());
		}

		protected abstract void OnSelect(CardSelectionRuntimeData selectionData);

		public override async Task MoveCardToBestDestination(Card card, bool playerDriven = true) { Debug.Log("Calling MoveCardToBestDestination in BaseMultiselectMoveBehaviour"); }
	}

}