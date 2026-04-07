namespace CardGameArchive
{
    using UnityEngine;

    public class DeckObject : MonoBehaviour, ITappable, ISaveable
    {
		private Deck deckData = null;

		SpriteRenderer sRenderer;

        void Awake()
        {
            sRenderer = GetComponent<SpriteRenderer>();
        }
        public void InitializeDeck(Deck deck)
		{
			deckData = deck;
		}

		public void OnTap()
		{
			BaseGameManager.Instance.OnDeckTapped(deckData);
		}

		public void SetVisible()
		{
			sRenderer.sprite = deckData.RemainingCards > 0 ? CardSpriteCollection.Instance.GetCardBack() : CardSpriteCollection.Instance.GetEmptyCard();
		}

		public class DeckSaveData : SaveData
		{

		}
		public SaveData Save()
		{
			throw new System.NotImplementedException();
		}

		public void Load(SaveData saveData)
		{
			throw new System.NotImplementedException();
		}
	}

}