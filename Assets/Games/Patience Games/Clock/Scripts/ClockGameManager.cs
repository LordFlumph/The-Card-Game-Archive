namespace CardGameArchive.Solitaire.Clock
{
	using CardGameArchive.Solitaire.Klondike;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

	public class ClockGameManager : KlondikeGameManager
	{
		Card activeCard = null;

		protected override void SetGame()
		{
			Rules = new ClockGameRules();
			Name = GameTerms.GameName.Clock;
		}
		protected override async Task StartGame()
		{
			int verificationCounter = 1;
			Deck deck = gameBoard.GetDeck();
			while (verificationCounter < 100)
			{
				deck.Shuffle(false);
				if (VerifyDeck())
				{
					break;
				}
				verificationCounter++;
			}

			Debug.Log("Verification attempts: " + verificationCounter);
			if (verificationCounter >= 100)
			{
				Debug.LogWarning("Failed to verify deck");
			}

			GameTaskManager.Instance.AddTask(gameBoard.GenerateCards());
			GameTaskManager.Instance.QueueTask(() => Task.Delay(500));

			await GameTaskManager.Instance.WhenAll();

			ZoneParent kingTableau = gameBoard.GetZoneParents(GameBoard.CardZone.Tableau)[^1];

			while (deck.RemainingCards > 0)
			{
				foreach (ZoneParent zone in gameBoard.GetZoneParents(GameBoard.CardZone.Tableau))
				{
					Card card = deck.Draw();

					if (zone != kingTableau)
					{
						GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, zone, canUndo: false, affectCardChain: false));
						await Awaitable.WaitForSecondsAsync(0.05f); 
					}
					else
					{
						GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, zone, teleport: true, canUndo: false, affectCardChain: false));
					}

					card.SetInteractable(false, false);
				}
			}

			GameTaskManager.Instance.QueueTask(() => Task.Delay(500));
			GameTaskManager.Instance.QueueTask(() => kingTableau.BottomCard.SetFlipped(true));
			await GameTaskManager.Instance.WhenAll();

			kingTableau.BottomCard.SetInteractable(true);
			activeCard = kingTableau.BottomCard;
		}
		protected override bool VerifyDeck()
		{
			// Since the odds of actually winning this game aren't high, and that is kind of the point, we don't want to remove unwinnable decks
			// We simply want to reduce the frequency of losing quickly
			// 1. Fail if all kings are within the first 20 cards
			// 2. Fail if the kings are within 5 cards of each other

			return true;
		}
		public override void OnDeckTapped(Deck deck){ }
		public override List<ZoneParent> GetPossibleMoves(Card card, bool simulation = false)
		{
			return null;
		}
		public override void AutoMoveAny() 
		{
			
		}
		public override async Task AutoMove(Card card, bool playerDriven = true)
		{
			if (!Rules.CanCardMove(card))
				return;

			GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, destination: GameBoard.CardZone.Foundation, index: Rules.GetRankValue(card.Rank)-1));
		}
		public override bool IsGameStuck()
		{
			return false;
		}
		protected override void OnCardMoveStart(GameBoard.CardMoveEvent eventData)
		{
			eventData.card.SetInteractable(false, false);

			ZoneParent linkedTableau = GetLinkedZone(eventData.to);
			activeCard = linkedTableau?.BottomCard;

			if (activeCard == null)
				return;

			GameTaskManager.Instance.AddTask(activeCard.SetFlipped(true));
			activeCard.SetInteractable(true);
		}
		protected override void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			AutoMoveAny();
		}

		ZoneParent GetLinkedZone(ZoneParent parent) => gameBoard.GetZoneParents(parent.Zone == GameBoard.CardZone.Tableau 
			? GameBoard.CardZone.Foundation : GameBoard.CardZone.Tableau) [gameBoard.GetZoneIndex(parent)];

		public override void Load(SaveData saveData)
		{
			base.Load(saveData);

			if (!loadFailed)
			{
				foreach (ZoneParent parent in gameBoard.AllZoneParents)
				{
					foreach (CardObject card in parent.Cards)
					{
						FeedbackManager.Instance.EnableCard(card);
					}
				}
			}
		}
	}
}
