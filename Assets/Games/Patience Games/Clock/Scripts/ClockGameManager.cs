namespace CardGameArchive.Solitaire.Clock
{
	using CardGameArchive.Solitaire.Klondike;
	using System.Collections.Generic;
	using System.Linq;
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
						GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, zone, canUndo: false, affectCardChain: false, timeToMove: 0.2f));
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
		public override void OnCardDropped(Card card)
		{
			bool actionExecuted = false;

			List<GameObject> objectsToIgnore = new();
			objectsToIgnore.AddRange(Rules.GetCardChain(card).Select(o => o.linkedObj.gameObject));

			RaycastHit2D[] hits = Physics2D.RaycastAll(card.linkedObj.transform.position, Vector3.forward, GameBoard.TopCardZ * 2);
			foreach (var hit in hits.OrderBy(o => o.distance))
			{
				if (objectsToIgnore.Contains(hit.collider.gameObject))
					continue;

				if (hit.collider.TryGetComponent(out ZoneParent zoneParent))
				{
					if (gameBoard.GetZoneIndex(zoneParent) == Rules.GetRankValue(card.Rank) - 1)
					{
						ZoneParent targetParent = zoneParent;
						if (zoneParent.Zone == GameBoard.CardZone.Tableau)
						{
							targetParent = GetLinkedZone(targetParent);
						}

						actionExecuted = true;
						GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, targetParent, timeToMove: 0.1f));
					}
					break;
				}

				else if (hit.collider.TryGetComponent(out CardObject otherCard))
				{
					ZoneParent otherZoneParent = otherCard.GetZoneParent();
					if (gameBoard.GetZoneIndex(zoneParent) == Rules.GetRankValue(card.Rank) - 1)
					{
						if (otherZoneParent.Zone == GameBoard.CardZone.Tableau)
							otherZoneParent = GetLinkedZone(otherZoneParent);


						actionExecuted = true;
						GameTaskManager.Instance.AddTask(gameBoard.MoveCard(card, otherZoneParent, timeToMove: 0.1f));
					}
					
					break;
				}
			}

			// No point in telling the player it was invalid if they clearly weren't trying to put it somewhere
			if (!actionExecuted && hits.Length > 0)
			{
				InvokeInvalidAction(card);
			}
		}
		public override List<ZoneParent> GetPossibleMoves(Card card, bool simulation = false)
		{
			return null;
		}
		public override void AutoMoveAny() 
		{
			if (activeCard == null)
				return;

			if (activeCard.Rank.ToString().ToLower() == activeCard.GetZoneParent().name.ToLower())
			{
				GameTaskManager.Instance.AddTask(Task.Delay(250));
				GameTaskManager.Instance.QueueTask(() => gameBoard.MoveCard(activeCard, destination: GameBoard.CardZone.Foundation, index: Rules.GetRankValue(activeCard.Rank) - 1, timeToMove: 0.1f));
			}
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
		}
		protected override void OnCardMoveFinish(GameBoard.CardMoveEvent eventData)
		{
			// If a Foundation is full (all 4 cards), squish them together
			if (eventData.to.Zone == GameBoard.CardZone.Foundation)
			{
				if (eventData.to.CardCount == 4)
					eventData.to.UseOperations = true;
			}

			ZoneParent linkedTableau = GetLinkedZone(eventData.to);
			activeCard = linkedTableau?.BottomCard;

			if (activeCard == null) // Game is over
				return;

			GameTaskManager.Instance.AddTask(activeCard.SetFlipped(true));
			activeCard.SetInteractable(true);
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
