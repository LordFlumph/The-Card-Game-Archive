namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "ScorelessScoreBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Score Behaviours/Scoreless")]
	public class ScorelessScoreBehaviour : BaseScoreBehaviour
	{
		public override int GetScore() => 0;
	}

}