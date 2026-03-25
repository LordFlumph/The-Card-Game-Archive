namespace CardGameArchive
{
	using UnityEngine;

	public class ZoneIdentifier : MonoBehaviour
	{
		[field: SerializeField] public GameBoard.CardZone Zone { get; private set; }
	}
}
