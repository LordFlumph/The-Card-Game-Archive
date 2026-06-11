namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	public abstract class BaseDeckVerificationBehaviour : ScriptableObject
	{
		[SerializeField] protected int maxVerificationAttempts = 50;
		public virtual void Verify()
		{
			int verificationCounter = 1;
			do
			{
				foreach (Deck deck in GameBoard.Instance.GetDecks())
				{
					deck.Shuffle(false);
				}

				if (VerifyDeck())
				{
					break;
				}

				verificationCounter++;

			} while (verificationCounter < maxVerificationAttempts);

			if (verificationCounter >= maxVerificationAttempts)
			{
				Debug.LogWarning("Failed to verify deck");
			}
			else
			{
				Debug.Log("Verification attempts: " + verificationCounter);
			}
		}
		protected abstract bool VerifyDeck();
	}

}