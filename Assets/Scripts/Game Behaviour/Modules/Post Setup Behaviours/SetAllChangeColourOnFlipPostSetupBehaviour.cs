namespace CardGameArchive.Behaviours
{
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "SetAllChangeColourOnFlipPostSetupBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Post Setup Behaviours/Set All Change Colour On Flip")]
	public class SetAllChangeColourOnFlipPostSetupBehaviour : BasePostSetupBehaviour
	{
		[SerializeField] bool changeColourOnFlip;
		public override async Task FinaliseBoard()
		{
			foreach (Card card in GameBoard.Instance.AllCards)
			{
				card.ChangeColourOnFlip = changeColourOnFlip;
			}
		}
	}

}