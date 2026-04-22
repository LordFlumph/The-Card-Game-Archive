namespace CardGameArchive
{
	using System;

	public class GameMove : ISaveable
	{
		public enum MoveType
		{
			CardFlipped,
			CardMoved,
			CardsDrawn,
			DeckShuffled,
			ZoneTransfer, // Move all cards from a zone to another
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
				saveData.cardID = cardData?.Data.ID ?? -1;
				saveData.contingent = contingent;
				return saveData;
			}

			public virtual void Load(SaveData saveData)
			{
				MoveSaveData moveSaveData = saveData as MoveSaveData;
				if (moveSaveData.cardID != -1)
					cardData = GameBoard.Instance.GetCardByID(moveSaveData.cardID);
				contingent = moveSaveData.contingent;
			}

			public void LoadFailed(string reason)
			{
				throw new NotImplementedException();
			}
		}
		public class CardFlippedData : MoveData
		{
			public bool flipped;

			public CardFlippedData() { }
			public CardFlippedData(Card cardData, bool flipped, bool contingent = false) : base(cardData, contingent)
			{
				this.flipped = flipped;
			}

			public class FlippedMoveSaveData : MoveSaveData
			{
				public bool flipped;
			}

			public override SaveData Save()
			{
				FlippedMoveSaveData saveData = new();
				saveData.CopyFrom(base.Save() as MoveSaveData);
				saveData.flipped = flipped;
				return saveData;
			}
			public override void Load(SaveData saveData)
			{
				base.Load(saveData);
				FlippedMoveSaveData flippedSaveData = saveData as FlippedMoveSaveData;
				flipped = flippedSaveData.flipped;
			}
		}
		public class CardMovedData : MoveData
		{
			public ZoneParent from;
			public ZoneParent to;

			public CardMovedData() { }
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
				saveData.fromIndex = GameBoard.Instance.GetZoneIndex(from);
				saveData.toIndex = GameBoard.Instance.GetZoneIndex(to);
				return saveData;
			}
			public override void Load(SaveData saveData)
			{
				base.Load(saveData);
				CardMovedSaveData movedSaveData = saveData as CardMovedSaveData;
				from = GameBoard.Instance.GetZoneParents(movedSaveData.fromZone)[movedSaveData.fromIndex];
				to = GameBoard.Instance.GetZoneParents(movedSaveData.toZone)[movedSaveData.toIndex];
			}
		}
		public class CardsDrawnData : MoveData
		{
			public int cardsDrawn;

			public CardsDrawnData() { }
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
				return saveData;
			}
			public override void Load(SaveData saveData)
			{
				base.Load(saveData);
				DrawMoveSaveData drawSaveData = saveData as DrawMoveSaveData;
				cardsDrawn = drawSaveData.cardsDrawn;
			}
		}
		public class ZoneTransferData : MoveData
		{
			public ZoneParent from, to;

			public ZoneTransferData() { }
			public ZoneTransferData(ZoneParent from, ZoneParent to, bool contingent = false) : base(null, contingent)
			{
				this.from = from;
				this.to = to;
			}

			public class ZoneTransferSaveData : MoveSaveData
			{
				public GameBoard.CardZone fromZone, toZone;
				public int fromIndex, toIndex;
			}
			public override SaveData Save()
			{
				ZoneTransferSaveData saveData = new();
				saveData.CopyFrom(base.Save() as MoveSaveData);
				saveData.fromZone = from.Zone;
				saveData.toZone = to.Zone;
				saveData.fromIndex = GameBoard.Instance.GetZoneIndex(from);
				saveData.toIndex = GameBoard.Instance.GetZoneIndex(to);
				return saveData;
			}
			public override void Load(SaveData saveData)
			{
				base.Load(saveData);
				ZoneTransferSaveData zoneTransferSaveData = saveData as ZoneTransferSaveData;
				from = GameBoard.Instance.GetZoneParents(zoneTransferSaveData.fromZone)[zoneTransferSaveData.fromIndex];
				to = GameBoard.Instance.GetZoneParents(zoneTransferSaveData.toZone)[zoneTransferSaveData.toIndex];
			}
		}

		public MoveData Data { get; private set; }

		public GameMove() { }
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
			try
			{
				GameMoveSaveData moveSaveData = saveData as GameMoveSaveData;

				type = moveSaveData.type;
				Data = moveSaveData.type switch
				{
					MoveType.CardFlipped => new CardFlippedData(),
					MoveType.CardMoved => new CardMovedData(),
					MoveType.CardsDrawn => new CardsDrawnData(),
					MoveType.DeckShuffled => new MoveData(),
					MoveType.ZoneTransfer => new ZoneTransferData(),
					_ => throw new Exception("Invalid move type in save data"),
				};
				Data.Load(moveSaveData.moveSaveData);
			}
			catch (Exception e)
			{
				LoadFailed(e.Message);
			}
		}

		public void LoadFailed(string reason)
		{
			BaseGameManager.Instance.LoadFailed(reason);
		}
	}

}