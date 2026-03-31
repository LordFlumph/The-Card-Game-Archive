namespace CardGameArchive
{
	public class GameMove
	{
		public enum MoveType
		{
			CardFlipped,
			CardMoved,
			CardsDrawn,
			DeckShuffled,
			WasteRecycled,
		}
		public MoveType type { get; private set; }

		/// <summary>
		/// Did this move occur because of another move? (e.g. card flipped because a card under it was moved
		/// </summary>
		public bool Contingent { get => Data.contingent; }

		public class MoveData
		{
			public Card cardData { get; private set; }
			public bool contingent { get; private set; }
			public MoveData(Card cardData = null, bool contingent = false)
			{
				this.cardData = cardData;
				this.contingent = contingent;
			}
		}
		public class CardFlippedData : MoveData
		{
			public bool flipped;

			public CardFlippedData(Card cardData, bool flipped, bool contingent = false) : base(cardData, contingent)
			{
				this.flipped = flipped;
			}
		}
		public class CardMovedData : MoveData
		{
			public ZoneParent from;
			public ZoneParent to;

			public CardMovedData(Card cardData, ZoneParent from, ZoneParent to, bool contingent = false) : base(cardData, contingent)
			{
				this.from = from;
				this.to = to;
			}
		}
		public class CardsDrawnData : MoveData
		{
			public int cardsDrawn;
			public CardsDrawnData(int cardsDrawn, bool contingent = false) : base(null, contingent)
			{
				this.cardsDrawn = cardsDrawn;
			}
		}
		public class WasteRecycledData : MoveData
		{
		}

		public MoveData Data { get; private set; }

		public GameMove(MoveType type, MoveData moveData)
		{
			this.type = type;
			Data = moveData;
		}
	}

}