namespace CardGameArchive
{
	using UnityEngine;


	public interface IDraggable
	{
		public bool CanDrag { get; set; }
		public void OnGrab();
		public void OnDrop();
	}
}