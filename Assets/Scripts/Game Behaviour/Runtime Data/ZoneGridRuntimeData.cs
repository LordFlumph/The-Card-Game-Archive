namespace CardGameArchive.Behaviours
{
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "ZoneGridRuntimeData", menuName = "Card Game Archive/Game Behaviour/Runtime Data/Zone Grid")]
	public class ZoneGridRuntimeData : BaseRuntimeData
	{
		[field: SerializeField] public GameBoard.CardZone CardZone { get; private set; }
		public enum GridType
		{
			Uniform,
			Pyramid
		};
		[SerializeField] GridType gridType;
		[SerializeField] bool overlapsVertically, overlapsHorizontally;
		List<List<ZoneParent>> grid = new();
		// Size is automatically determined based on ZoneParent positions

		public override void Initialise()
		{
			BuildGrid();
		}

		void BuildGrid()
		{
			grid.Clear();

			List<ZoneParent> allTargetZones = GameBoard.Instance.GetZoneParents(CardZone);

			grid.Add(new List<ZoneParent>() { allTargetZones[0] });
			for (int i = 1; i < allTargetZones.Count; i++)
			{
				// Check if a row for this already exists
				// If it does, then determine where in that row this should be based on X position
				// If it doesn't, then determine where in the grid this new row should be

				int rowIndex = FindRowIndex(allTargetZones[i], out bool newRow);
				if (newRow)
				{
					grid.Insert(rowIndex, new List<ZoneParent>() { allTargetZones[i] });
				}
				else
				{
					int columnIndex = FindColumnIndex(allTargetZones[i], rowIndex);

					if (columnIndex == grid[rowIndex].Count)
					{
						grid[rowIndex].Add(allTargetZones[i]);
					}
					else
					{
						grid[rowIndex].Insert(columnIndex, allTargetZones[i]);
					}
				}
			}
		}

		int FindRowIndex(ZoneParent zoneParent, out bool newRow)
		{
			newRow = false;
			for (int i = 0; i < grid.Count; i++)
			{
				if (Mathf.Abs(grid[i][0].transform.position.y - zoneParent.transform.position.y) < 0.1f)
				{
					return i;
				}
				else if (grid[i][0].transform.position.y < zoneParent.transform.position.y)
				{
					newRow = true;
					return i;
				}
			}

			// If we reach here, then this zoneParent is below all existing rows
			newRow = true;
			return grid.Count;
		}

		int FindColumnIndex(ZoneParent zoneParent, int rowIndex)
		{
			for (int i = 0; i < grid[rowIndex].Count; i++)
			{
				if (zoneParent.transform.position.x < grid[rowIndex][i].transform.position.x)
				{
					return i;
				}
			}

			return grid[rowIndex].Count;
		}

		/// <summary>
		/// Determine if the given ZoneParent is covered by any other ZoneParent in the grid. This is based on the grid type and the overlaps settings. Empty ZoneParents are ignored
		/// </summary>
		/// <returns>Whether the given ZoneParent is covered by a non-empty ZoneParent</returns>
		public bool IsZoneCovered(ZoneParent zone) => GetCoveringZones(zone).Count > 0;

		public (int row, int column) GetZoneGridIndex(ZoneParent zone)
		{
			for (int i = 0; i < grid.Count; i++)
			{
				for (int j = 0; j < grid[i].Count; j++)
				{
					if (grid[i][j] == zone)
					{
						return (i, j);
					}
				}
			}

			return (-1, -1);
		}

		public bool IsZoneCoveredByZone(ZoneParent targetZone, ZoneParent coveringZone) => GetAllCoveringZones(targetZone).Contains(coveringZone);

		/// <summary>
		/// Returns a list of ZoneParents that are covering the given ZoneParent, based on the grid type and overlaps settings. Empty ZoneParents are ignored
		/// </summary>
		List<ZoneParent> GetCoveringZones(ZoneParent zone)
		{
			if (!overlapsVertically && !overlapsHorizontally)
				return new();

			if (zone.Zone != this.CardZone)
				return new();

			(int row, int column) = GetZoneGridIndex(zone);

			List<(int row, int column)> cellsToCheck = new();

			switch (gridType)
			{
				case GridType.Uniform:
					if (overlapsVertically)
					{
						cellsToCheck.Add((row + 1, column));
					}
					if (overlapsHorizontally)
					{
						cellsToCheck.Add((row, column + 1));
					}

					if (overlapsVertically && overlapsHorizontally)
					{
						cellsToCheck.Add((row + 1, column + 1));
					}
					break;

				case GridType.Pyramid:
					if (overlapsVertically && row + 1 < grid.Count)
					{
						for (int i = 1; i < grid[row + 1].Count; i++)
						{
							if (grid[row + 1][i - 1].transform.position.x < zone.transform.position.x && grid[row + 1][i].transform.position.x > zone.transform.position.x)
							{
								cellsToCheck.Add((row + 1, i - 1));
								cellsToCheck.Add((row + 1, i));
								break;
							}
						}
					}
					break;
			}

			List<ZoneParent> coveringZones = new();

			foreach (var cell in cellsToCheck)
			{
				if (cell.row < 0 || cell.row >= grid.Count
					|| cell.column < 0 || cell.column >= grid[cell.row].Count)
				{
					continue;
				}


				if (grid[cell.row][cell.column].CardCount > 0)
					coveringZones.Add(grid[cell.row][cell.column]);
			}

			return coveringZones;
		}
		/// <summary>
		/// Returns a list of all ZoneParents that are covering the given ZoneParent, as well as all the Zones covering those recursively
		/// </summary>
		List<ZoneParent> GetAllCoveringZones(ZoneParent zone)
		{
			List<ZoneParent> coveringZones = new();
			foreach (var coveringZone in GetCoveringZones(zone))
			{
				if (coveringZones.Contains(coveringZone))
					continue;

				coveringZones.Add(coveringZone);
				coveringZones.AddRange(GetAllCoveringZones(coveringZone));
			}
			return coveringZones;
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