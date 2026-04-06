namespace CardGameArchive
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using static CardGameArchive.Card;

	public class CardSpriteCollection : MonoBehaviour
	{
		public static CardSpriteCollection Instance { get; private set; }

		[Serializable]
		private class SpriteData
		{
			public CardData data;
			public Sprite sprite;
			
			public SpriteData(CardData data, Sprite sprite = null)
			{
				this.data = data;
				this.sprite = sprite;
			}
		}

		[SerializeField] private List<SpriteData> cardSprites;
		[SerializeField] private Sprite cardBack;
		[SerializeField] private Sprite emptyCard;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}				
		}

		public Sprite this[CardData data] => cardSprites.Find(o => o.data.rank == data.rank && o.data.suit == data.suit).sprite;

		public Sprite GetCardBack() => cardBack;
		public Sprite GetEmptyCard() => emptyCard;
	}
}
