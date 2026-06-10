namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	/// <summary>
	/// When deck is tapped, it runs a DealSetupBehaviour a set number of times
	/// </summary>
	[CreateAssetMenu(fileName = "RunDealDeckBehaviour", menuName = "Card Game Archive/Game Behaviour/Deck Behaviours/Run Deal Behaviour")]
	public class RunDealDeckBehaviour : BaseDeckBehaviour
	{
		[SerializeField] int timesToDeal = 1;
		[SerializeField] BaseGameDealBehaviour dealBehaviour;
		protected override void OnDeckTapped(Deck deck)
		{
			int deal = 0;
			while (deal < timesToDeal)
			{
				dealBehaviour.DealCards();
				deal++;
			}
		}
	}

}