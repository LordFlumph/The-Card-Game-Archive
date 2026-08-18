namespace CardGameArchive.Behaviours
{
    using UnityEngine;

	public class ZoneGridRuntimeData : BaseRuntimeData
	{
		[SerializeField] GameBoard.CardZone zone;
		[SerializeField] bool overlapsVertically, overlapsHorizontally;


		// Type of grid (uniform, triangular, etc)
		// Size is automatically determined based on ZoneParent positions

		public override void Initialise()
		{
			
		}

		public override SaveData Save()
		{
			return new EmptySaveData();
		}
		public override void Load(SaveData saveData)
		{
			Initialise();
		}
	}

}