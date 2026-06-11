namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "FlipBottomCardOfZonePostSetupBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Post Setup Behaviours/Flip Bottom Card of Zone")]
	public class FlipBottomCardOfZonePostSetupBehaviour : BasePostSetupBehaviour
	{
		[System.Serializable] struct ParentData { public GameBoard.CardZone zone; public int index; }
		[SerializeField] List<ParentData> zonesToFlip;
		[SerializeField] bool setInteractable = true;

		public override async Task FinaliseBoard()
		{
			List<Task> tasks = new();
			foreach (var data in zonesToFlip)
			{
				ZoneParent parent = GameBoard.Instance.GetZoneParent(data.zone, data.index);
				if (parent == null || parent.CardCount == 0)
					continue;

				if (setInteractable)
					parent.BottomCard.SetInteractable(true);
				tasks.Add(parent.BottomCard.SetFlipped(true));
			}
			await Task.WhenAll(tasks);
		}
	}
}