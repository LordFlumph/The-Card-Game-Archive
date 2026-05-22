using System.Threading.Tasks;

namespace CardGameArchive
{
	using UnityEngine;
	[CreateAssetMenu(fileName = "TableauDealSetupBehaviour", menuName = "Game Behaviour : ScriptableObjects/Game Setup Behaviour : ScriptableObjects/Tableau Deal")]
	public class TableauDealSetupBehaviour : IGameSetupBehaviour
	{
		int cardsPerTableau;
		int faceUpCardsPerTableau;
		public override async Task DealCards()
		{

		}

		public override void FinaliseBoard()
		{

		}
	}
}
