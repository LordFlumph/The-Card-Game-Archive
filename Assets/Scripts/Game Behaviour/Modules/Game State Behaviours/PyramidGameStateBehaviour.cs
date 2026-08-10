namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "PyramidGameStateBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Game State Behaviours/Pyramid")]
	public class PyramidGameStateBehaviour : BaseGameStateBehaviour
	{
		public override bool IsGameStuck()
		{
			Debug.Log("IsGameStuck Unimplemented");
			if (StandardGameManager.Instance.Variant == GameTerms.GameVariant.PyramidTraditional)
			{
				// Game is stuck if all are true
				// 1. No cards remain in stock
				// 2. There are no possible pairs between all cards in Tableau and Waste
			}
			else
			{
				// Game is stuck if there are no pairs in the Tableau, or between the Tableau and ANY card in the Waste or Stock
			}

			return false;
		}
	}

}