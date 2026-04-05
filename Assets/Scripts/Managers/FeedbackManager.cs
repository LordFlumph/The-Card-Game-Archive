namespace CardGameArchive
{
	using System.Threading.Tasks;
    using System.Collections.Generic;
	using UnityEngine;

    /// <summary>
    /// Handles all code related to managing visual feedback for the player
    /// </summary>
    public class FeedbackManager : MonoBehaviour
    {
        public static FeedbackManager Instance { get; private set; }

		[Header("Card Shake")]
        [SerializeField] float shakeDuration = 0.2f;
        [SerializeField] float shakeSpeed = 2.5f;
        [SerializeField] float shakeAngle = 5;

        [Header("Card Highlight")]
        [SerializeField] Color highlightColour = Color.white;
        [SerializeField] float highlightSize;
        List<CardObject> highlightedCards;

		void Awake()
		{
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
		}

		public async void OnInvalidAction(Card card)
        {
            GameTaskManager.Instance.AddTask(ShakeCard(card.linkedObj));                
		}

        async Task ShakeCard(CardObject card)
        {
            if (card == null)
                return;

			float shakeTimer = shakeDuration;
            bool clockwise = true;
            while (shakeTimer > 0)
            {
                card.transform.rotation *= Quaternion.Euler(0, 0, (clockwise ? shakeSpeed : -shakeSpeed) * Time.deltaTime);

                if (clockwise && card.transform.rotation.eulerAngles.z <= 180 && card.transform.rotation.eulerAngles.z > shakeAngle)
                    clockwise = false;
                else if (!clockwise && card.transform.rotation.eulerAngles.z > 180 && card.transform.rotation.eulerAngles.z < 360 - shakeAngle)
                    clockwise = true;

                shakeTimer -= Time.deltaTime;

				await Task.Yield();
            }

            card.transform.rotation = Quaternion.identity;
        }
    

        public void HighlightCard(CardObject card)
        {
            if (card == null)
                return;

            // Highlight card
            highlightedCards.Add(card);
        }

		public void ClearHighlights()
		{
            for (int i = highlightedCards.Count - 1; i >= 0; i--)
            {
                // Remove highlight
                highlightedCards.RemoveAt(i);
			}
		}
		public void ClearHighlight(CardObject card)
		{
            if (card != null && highlightedCards.Contains(card))
            {
                // Remove highlight
                highlightedCards.Remove(card);
            }
		}
	}

}