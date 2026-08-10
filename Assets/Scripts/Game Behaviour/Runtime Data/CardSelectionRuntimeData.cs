namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "CardSelectionRuntimeData", menuName = "Card Game Archive/Game Behaviour/Runtime Data/Card Selection Data")]
	public sealed class CardSelectionRuntimeData : BaseRuntimeData
	{
		List<CardObject> selectedCards = new();
		public int SelectedCardCount { get { ValidateSelection(); return selectedCards.Count; } }

		[SerializeField] public bool highlightSelectedCards = true;
		[SerializeField] public Color highlightColour;

		public CardObject this[int index] => GetCard(index);

		public CardObject GetCard(int index)
		{
			if (index < 0 || index >= selectedCards.Count)
				return null;
			return selectedCards[index];
		}
		public List<CardObject> GetSelectedCards() => selectedCards;

		public void SelectCard(Card card) => SelectCard(card.linkedObj);
		public void SelectCard(CardObject card)
		{
			ValidateSelection();

			selectedCards.Add(card);

			if (highlightSelectedCards)
				FeedbackManager.Instance.Highlight(card, highlightColour);
		}

		public void DeselectCard(Card card) => DeselectCard(card.linkedObj);
		public void DeselectCard(CardObject card)
		{
			selectedCards.Remove(card);

			if (highlightSelectedCards)
				FeedbackManager.Instance.ClearHighlight(card);
		}

		public void DeselectAll()
		{
			for (int i = selectedCards.Count-1; i >= 0; i--)
			{
				DeselectCard(selectedCards[i]);
			}
		}

		public bool IsCardSelected(Card card) => IsCardSelected(card.linkedObj);
		public bool IsCardSelected(CardObject card)
		{
			ValidateSelection();
			return selectedCards.Contains(card);
		}

		void ValidateSelection()
		{
			for (int i = selectedCards.Count - 1; i >= 0; i--)
			{
				if (selectedCards[i].Data.Interactable == false)
					DeselectCard(selectedCards[i]);
			}
		}

		public class CardSelectionRuntimeSaveData : SaveData
		{
			public List<int> selectedCardIds = new();
		}
		public override SaveData Save()
		{
			CardSelectionRuntimeSaveData saveData = new();
			foreach (CardObject card in selectedCards)
			{
				saveData.selectedCardIds.Add(card.Data.ID);
			}
			return saveData;
		}

		public override void Load(SaveData saveData)
		{
			CardSelectionRuntimeSaveData cardSelectionSaveData = saveData as CardSelectionRuntimeSaveData;
			if (cardSelectionSaveData == null)
			{
				LoadFailed("Invalid save data type for CardSelectionRuntimeData.");
				return;
			}

			foreach (int ID in cardSelectionSaveData.selectedCardIds)
			{
				Card card = GameBoard.Instance.GetCardByID(ID);
				if (card != null)
				{
					selectedCards.Add(card.linkedObj);
					
					if (highlightSelectedCards)
					{
						foreach (CardObject cardObj in selectedCards)
						{
							FeedbackManager.Instance.Highlight(cardObj, highlightColour);
						}
					}
				}
				else
				{
					LoadFailed($"Card #{ID} could not be selected");
				}
			}
		}
	}
}