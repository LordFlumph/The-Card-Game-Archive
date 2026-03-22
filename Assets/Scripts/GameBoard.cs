namespace CardGameArchive
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public class GameBoard : MonoBehaviour
	{
		public static GameBoard Instance { get; private set; }

		[SerializeField] private GameObject cardPrefab;

		[SerializeField] private Transform deckTransform;

		[SerializeField] private List<Transform> tableauParents;

		[SerializeField] private float cardSpacing;

		[SerializeField] private float cardMoveSpeed;

		void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		public async void PlaceCard(Card card, int column, int row = -1, bool fromDeck = false, bool teleport = false)
		{
			if (card == null)
			{
				Debug.LogError("Card is null");
				return;
			}

			if (column < 0 || column >= tableauParents.Count)
			{
				Debug.LogError($"Invalid column index: {column}");
				return;
			}

			bool setInteractable = card.interactable;

			card.interactable = false;

			if (row < 0) row = tableauParents[column].childCount;
			if (row > tableauParents.Count) row = tableauParents[column].childCount;

			if (card.linkedObj == null)
			{
				card.linkedObj = Instantiate(cardPrefab, tableauParents[column]);
			}
			else
			{
				card.linkedObj.transform.SetParent(tableauParents[column]);
			}

			if (fromDeck)
			{
				card.linkedObj.transform.position = deckTransform.position;
			}

			Vector3 targetLocation = tableauParents[column].position + (Vector3.down * cardSpacing * row);
			if (teleport)
			{
				card.linkedObj.transform.position = targetLocation;
			}
			else
			{
				await MoveCard(card.linkedObj, targetLocation);
			}

			card.interactable = setInteractable;
		}

		async Task MoveCard(GameObject obj, Vector3 targetPosition)
		{
			while (Vector3.Distance(obj.transform.position, targetPosition) > cardMoveSpeed * 0.1f)
			{
				obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, cardMoveSpeed * Time.deltaTime);
				await Task.Yield();
			}
			obj.transform.position = targetPosition;
		}
	}
}