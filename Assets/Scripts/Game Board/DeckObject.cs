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

		[SerializeField] bool showEmptySprite = true;

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
			if (Data.RemainingCards > 0)
			{
				sRenderer.sprite = CardSpriteCollection.Instance.GetCardBack();
			}
			else
			{
				sRenderer.sprite = showEmptySprite ? CardSpriteCollection.Instance.GetEmptyCard() : null;
			}
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
					await Awaitable.WaitForSecondsAsync(animator.GetCurrentAnimatorStateInfo(0).length);
				}
			}			
		}
	}

}