namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "EnableZoneColourPostLoadBehaviour", menuName = "Card Game Archive/Game Behaviour/Post Load Behaviours/Enable Zone Colour")]
	public class EnableZoneColourPostLoadBehaviour : BasePostLoadBehaviour
	{
		[SerializeField] List<GameBoard.CardZone> zones;
		public override bool PostLoad(SaveData saveData)
		{
			foreach (ZoneParent parent in GameBoard.Instance.AllZoneParents)
			{
				if (zones.Contains(parent.Zone))
				{
					foreach (CardObject card in parent.Cards)
					{
						FeedbackManager.Instance.EnableCard(card);
					}
				}				
			}

			return true;
		}
	}

}