namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "ValueHolderRuntimeData", menuName = "Card Game Archive/Game Behaviour/Runtime Data/Value Holder")]
	public sealed class ValueHolderRuntimeData : BaseRuntimeData
	{
		public enum Identifier
		{
			DealToWasteLoopValue
		}
		[field: SerializeField] public Identifier identifier { get; private set; }
		object RawValue { get; set; }

		public override void Initialise()
		{
			RawValue = identifier switch
			{
				Identifier.DealToWasteLoopValue => 0,
				_ => throw new System.NotImplementedException("No default value set for identifier " + identifier)
			};
		}

		public T GetValue<T>() => RawValue is T value ? value : default;
		public void SetValue<T>(T value) => RawValue = value;

		public class ValueHolderSaveData : SaveData
		{
			public ValueHolderSaveData(object value) { this.value = value; }
			public object value;
		}
		public override SaveData Save() => new ValueHolderSaveData(RawValue);

		public override void Load(SaveData saveData)
		{
			ValueHolderSaveData data = saveData as ValueHolderSaveData;
			if (data == null)
			{
				LoadFailed("SaveData is not of type ValueHolderSaveData");
				return;
			}

			RawValue = data.value;
		}
	}

}