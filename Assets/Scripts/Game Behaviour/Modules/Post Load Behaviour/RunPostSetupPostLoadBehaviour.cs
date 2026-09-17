namespace CardGameArchive.Behaviours
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "RunPostSetupPostLoadBehaviour", menuName = "Card Game Archive/Game Behaviour/Modules/Post Load Behaviours/Run Post Setup Behaviour")]
	public class RunPostSetupPostLoadBehaviour : BasePostLoadBehaviour
	{
		[SerializeField] BasePostSetupBehaviour postSetupBehaviour;
		public override bool PostLoad(SaveData saveData)
		{
			GameTaskManager.Instance.AddTask(postSetupBehaviour.FinaliseBoard());
			return true;
		}
	}

}