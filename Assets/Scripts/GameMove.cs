namespace CardGameArchive
{
	public class GameMove : ISaveable
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

		public class MoveData : ISaveable
		{
			public Card cardData { get; private set; }
			public bool contingent { get; private set; }
			public MoveData(Card cardData = null, bool contingent = false)
			{
				this.cardData = cardData;
				this.contingent = contingent;
			}

			public class MoveSaveData : SaveData
			{
				public int cardID;
				public bool contingent;

				public void CopyFrom(MoveSaveData other)
				{
					cardID = other.cardID;
					contingent = other.contingent;
				}
			}

			public virtual SaveData Save()
			{
				MoveSaveData saveData = new();
				saveData.cardID = cardData.Data.ID;
				saveData.contingent = contingent;
				return saveData;
			}

			public virtual void Load(SaveData saveData)
			{
				throw new System.NotImplementedException();
			}
		}
		public class CardFlippedData : MoveData
		{
			public bool flipped;

			public CardFlippedData(Card cardData, bool flipped, bool contingent = false) : base(cardData, contingent)
			{
				this.flipped = flipped;
			}

			public class FlippedMoveSaveData : MoveSaveData
			{
				public bool fipped;
			}

			public override SaveData Save()
			{
				FlippedMoveSaveData saveData = new();
				saveData.CopyFrom(base.Save() as MoveSaveData);
				saveData.fipped = flipped;
				return saveData;
			}
			public override void Load(SaveData saveData)
			{
				base.Load(saveData);
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

			public class CardMovedSaveData : MoveSaveData
			{
				public GameBoard.CardZone fromZone, toZone;
				public int fromIndex, toIndex;
			}

			public override SaveData Save()
			{
				CardMovedSaveData saveData = new();
				saveData.CopyFrom(base.Save() as MoveSaveData);
				saveData.fromZone = from.Zone;
				saveData.toZone = to.Zone;
				saveData.fromIndex = GameBoard.Instance.GetZoneParents(from.Zone).IndexOf(from);
				saveData.toIndex = GameBoard.Instance.GetZoneParents(to.Zone).IndexOf(to);
				return saveData;
			}
			public override void Load(SaveData saveData)
			{
				
			}
		}
		public class CardsDrawnData : MoveData
		{
			public int cardsDrawn;
			public CardsDrawnData(int cardsDrawn, bool contingent = false) : base(null, contingent)
			{
				this.cardsDrawn = cardsDrawn;
			}

			public class DrawMoveSaveData : MoveSaveData
			{
				public int cardsDrawn;
			}

			public override SaveData Save()
			{
				DrawMoveSaveData saveData = new();
				saveData.CopyFrom(base.Save() as MoveSaveData);
				saveData.cardsDrawn = cardsDrawn;
				return base.Save();
			}
			public override void Load(SaveData saveData)
			{

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

		public class GameMoveSaveData : SaveData
		{
			public MoveType type;
			public SaveData moveSaveData;
		}

		public SaveData Save()
		{
			GameMoveSaveData saveData = new();
			saveData.type = type;
			saveData.moveSaveData = Data.Save();
			return saveData;
		}

		public void Load(SaveData saveData)
		{
			throw new System.NotImplementedException();
		}
	}

}