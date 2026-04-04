namespace CardGameArchive
{
	using System.Threading.Tasks;
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

		void Awake()
		{
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
		}

		public async void OnInvalidAction(Card card)
        {
            bool interactable = card.Interactable;               

            card.SetInteractable(false);

			await ShakeCard(card.linkedObj);

            card.SetInteractable(interactable);
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
    }

}