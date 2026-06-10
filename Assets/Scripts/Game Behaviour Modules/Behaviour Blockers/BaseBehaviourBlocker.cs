namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	public abstract class BaseBehaviourBlocker : ScriptableObject
	{
		/// <summary>
		/// Checks a condition to determine whether a behaviour should be blocked
		/// </summary>
		/// <returns>Whether the behaviour should be blocked</returns>
		public abstract bool BlockBehaviour();

		/// <summary>
		/// Invoked when the behaviour is blocked, useful for feedback regarding the block
		/// </summary>
		protected virtual void OnBehaviourBlocked<T>(T context) { }
	}
}
