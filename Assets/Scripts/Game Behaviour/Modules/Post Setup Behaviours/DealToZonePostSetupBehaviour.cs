namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	[CreateAssetMenu(fileName = "DealToZonePostSetupBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Post Setup Behaviours/Deal To Zone Behaviour")]
    public class DealToZonePostSetupBehaviour : BasePostSetupBehaviour
    {
		[SerializeField] GameBoard.CardZone zone;
		[SerializeField] int numberToDeal = 1;
		[SerializeField] int flippedCards = 1;

		public async override Task FinaliseBoard()
		{
			List<Task> tasks = new List<Task>();

			Deck deck = GameBoard.Instance.GetDeck();
			for (int i = 0; i < numberToDeal; i++)
			{
				if (deck.RemainingCards == 0) // If this happens, then it is a mistake so we log it as an error
				{
					Debug.LogError("Attempting to deal more cards than remaining in deck");
					break;
				}

				Card card = deck.Draw();
				tasks.Add(GameBoard.Instance.MoveCard(card, zone, canUndo: false));

				if (numberToDeal-1-i < flippedCards)
				{
					tasks.Add(card.SetFlipped(true));
				}
			}

			await Task.WhenAll(tasks);
		}
    }

}