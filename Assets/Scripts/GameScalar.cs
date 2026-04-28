namespace CardGameArchive
{
	using UnityEngine;

	/// <summary>
	/// Manages scaling the game board to fit different screen sizes
	/// </summary>
	public class GameScalar : MonoBehaviour
	{
		[SerializeField] GameBoard gameBoard;

		[SerializeField] float defaultAspect = 0.45f;
		[SerializeField] float maxScale = 1.4f;
		float currentAspect;

		Camera mainCamera;

		void Awake()
		{
			mainCamera = Camera.main;
		}

		void Update()
		{
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
			else if (Mathf.Abs(currentAspect - mainCamera.aspect) >= 0.01)
			{
				SetGameScale();
			}
		}

		void SetGameScale()
		{
			if (gameBoard == null)
				gameBoard = FindAnyObjectByType<GameBoard>(FindObjectsInactive.Include);

			if (gameBoard == null)
				return;

			currentAspect = mainCamera.aspect < 1 ? mainCamera.aspect : 1 / mainCamera.aspect;

			float newScale = Mathf.Min(currentAspect / defaultAspect, maxScale);

			gameBoard.transform.localScale = new Vector3(newScale, newScale, 1);
		}
	}
}