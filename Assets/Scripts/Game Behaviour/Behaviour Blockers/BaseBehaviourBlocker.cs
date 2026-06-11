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
		protected abstract void OnBehaviourBlocked(object context);
	}

	public abstract class BaseBehaviourBlocker<T> : BaseBehaviourBlocker
	{
		/// <summary>
		/// Invoked when the behaviour is blocked, useful for feedback regarding the block
		/// </summary>
		protected override void OnBehaviourBlocked(object context)
		{
			if (context is T typedContext)
			{
				OnBehaviourBlocked(typedContext);
			}
			else
			{
				Debug.LogError($"Invalid context type for behaviour blocker {name}. Expected {typeof(T)}, got {context.GetType()}");
			}
		}
		protected virtual void OnBehaviourBlocked(T context) { }
	}
}
