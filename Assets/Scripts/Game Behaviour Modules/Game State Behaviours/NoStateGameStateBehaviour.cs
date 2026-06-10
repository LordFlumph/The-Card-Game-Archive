namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "NoStateGameStateBehaviour", menuName = "Card Game Archive/Game Behaviour/Game State Behaviours/No State")]
	public class NoStateGameStateBehaviour : BaseGameStateBehaviour
	{
		public override bool IsGameStuck() => false;
	}

}