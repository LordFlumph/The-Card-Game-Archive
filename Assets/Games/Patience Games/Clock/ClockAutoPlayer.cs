#if UNITY_EDITOR
namespace CardGameArchive
{
	using System.Linq;
	using UnityEngine;

	public class ClockAutoPlayer : MonoBehaviour
	{
		void Update()
		{
			if (GameTaskManager.Instance.TaskCount == 0)
			{
				if (!StandardGameManager.Instance.GamePlaying && BaseGameRules.ActiveRules.IsLossConditionAchieved())
					StandardGameManager.Instance.RestartGame();
				else
					StandardGameManager.Instance.OnCardTapped(GameBoard.Instance.AllCards.FirstOrDefault(o => o.Interactable));

				if (!BaseGameRules.ActiveRules.IsWinConditionAchieved())
					PopupMenuManager.Instance.FadePopups();
			}
		}
	}

} 
#endif