namespace CardGameArchive
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class CardSpriteCollection : MonoBehaviour
	{
		public static CardSpriteCollection Instance { get; private set; }

		[Serializable]
		private class SpriteData
		{
			public Card.CardData data;
			public Sprite sprite;
		}

		[SerializeField] private List<SpriteData> cardSprites = new();		

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		public Sprite this[Card.CardData data] => cardSprites.Find(o => o.data.Equals(data)).sprite;
		public Sprite this[Card.CardValue value, Card.CardSuit suit] => cardSprites.Find(o => o.data.Equals(new Card.CardData(value, suit))).sprite;

	}
}
