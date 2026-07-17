namespace CardGameArchive.Behaviours
{
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "DisableDraggablesPostSetupBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Post Setup Behaviours/Disable Draggables")]
	public class DisableDraggablesPostSetupBehaviour : BasePostSetupBehaviour
	{
		public override async Task FinaliseBoard()
		{
			var draggables = FindObjectsByType<CardObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			foreach (var draggable in draggables)
			{
				draggable.CanDrag = false;
			}
		}
	}

}