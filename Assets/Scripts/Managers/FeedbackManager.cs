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

        [Header("Card Disabling")]
        [SerializeField] Color disabledColour;
        [SerializeField] float disableFadeTime = 0.1f;

        [Header("Card Highlight")]
        [SerializeField] GameObject highlightPrefab;
        [SerializeField] float highlightFadeTime = 0.2f;
        Dictionary<GameObject, SpriteRenderer> highlightedObjects = new();

        [field: SerializeField] public Color InvalidColour { get; private set; }
        [field: SerializeField] public Color AttentionColour { get; private set; }

		void Awake()
		{
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
		}

		public async void OnInvalidAction(Card card)
        {
            if (card?.linkedObj != null)
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

				await Awaitable.NextFrameAsync();
            }

            card.transform.localRotation = Quaternion.identity;
        }
        
        public void EnableCard(CardObject card)
        {
            card.sRenderer.LerpColor(disableFadeTime, Color.white);
		}
        public void DisableCard(CardObject card)
        {
			card.sRenderer.LerpColor(disableFadeTime, disabledColour);
        }

        public void Highlight(GameObject card, Color color, bool fade = true)
        {
            if (card == null)
                return;

            if (highlightedObjects.ContainsKey(card))
                return;

            // Highlight card
            SpriteRenderer highlight = Instantiate(highlightPrefab, card.transform).GetComponent<SpriteRenderer>();
            if (fade)
            {
                highlight.color = new Color(color.r, color.g, color.b, 0);
                highlight.FadeIn(highlightFadeTime, color);
            }
            else
            {
                highlight.color = color;
            }
            highlightedObjects.Add(card, highlight);
        }

        public async void PulseHighlight(GameObject card, Color color, float duration = 0.5f)
        {
            if (card == null)
                return;
            
            Highlight(card, color);
            await Awaitable.WaitForSecondsAsync(duration);
            ClearHighlight(card);
        }

		public void ClearHighlights()
		{
            foreach (var entry in highlightedObjects)
            {
                entry.Value.FadeOut(highlightFadeTime, true);
			}

            highlightedObjects.Clear();
		}
		public void ClearHighlight(GameObject card, bool fade = true)
		{
            if (card != null && highlightedObjects.TryGetValue(card, out SpriteRenderer renderer))
            {
                renderer.FadeOut(highlightFadeTime, true);
                highlightedObjects.Remove(card);
			}
		}
	}

}