namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	public abstract class BaseCondition : ScriptableObject
	{
		public abstract bool ConditionMet(object context);
	}
	public abstract class BaseCondition<T> : BaseCondition
	{
		public override bool ConditionMet(object context)
		{
			if (context is T typedContext)
			{
				return ConditionMet(typedContext);
			}
			else
			{
				Debug.LogError($"Invalid context type for condition {name}. Expected {typeof(T)}, got {context.GetType()}");
				return false;
			}
		}
		public abstract bool ConditionMet(T context);

	}
}