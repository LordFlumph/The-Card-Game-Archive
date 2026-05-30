namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	public class FoundationEnableColourPostLoadBehaviour : BasePostLoadBehaviour
	{
		public override bool PostLoad(SaveData saveData)
		{
			foreach (ZoneParent parent in GameBoard.Instance.GetZoneParents(GameBoard.CardZone.Foundation))
			{
				foreach (CardObject card in parent.Cards)
				{
					FeedbackManager.Instance.EnableCard(card);
				}
			}

			return true;
		}
	}

}