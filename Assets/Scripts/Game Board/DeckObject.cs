namespace CardGameArchive
{
	using System.Threading.Tasks;
    using UnityEngine;

	public class DeckObject : MonoBehaviour, ITappable
	{
		[SerializeField] Deck.DeckType deckType;
		public Deck Data { get; private set; } = new();

		[SerializeField] Animator animator;
		[SerializeField] int shuffleLoops = 3;

		SpriteRenderer sRenderer;

        void Awake()
        {
            sRenderer = GetComponent<SpriteRenderer>();
			InitializeDeck();
		}
        void InitializeDeck()
		{
			Data.Initialise(deckType, this);
		}

		public void OnTap()
		{
			BaseGameManager.Instance.OnDeckTapped(Data);
		}

		public void SetVisible()
		{
			sRenderer.sprite = Data.RemainingCards > 0 ? CardSpriteCollection.Instance.GetCardBack() : CardSpriteCollection.Instance.GetEmptyCard();
		}

		public async Task OnShuffle(bool visual)
		{
			return;
			ZoneParent zoneParent = GetComponent<ZoneParent>();
			zoneParent.RemoveAllCards();
			foreach (Card card in Data.Cards)
			{
				GameBoard.Instance.MoveCard(card, zoneParent, teleport: true, canUndo: false);
			}

			if (visual)
			{
				for (int i = 0; i < shuffleLoops; i++)
				{
					animator.Play(0);
					await Task.Delay((int)(animator.GetCurrentAnimatorStateInfo(0).length * 1000));
				}
			}			
		}
	}

}