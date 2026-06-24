namespace CardGameArchive.MainMenu
{
	using System.Threading.Tasks;
	using TMPro;
	using UnityEngine;

	public class SearchBar : MonoBehaviour
    {
		Animator animator;

		public bool IsDropdownOpen { get; private set; }

		bool animating;

		[SerializeField] TMP_InputField searchBar;

		void Awake()
		{
			animator = GetComponent<Animator>();
		}

		public void ToggleDropdown()
		{
			if (animating)
				return;

			animator.Play(IsDropdownOpen ? "CloseSearchBar" : "OpenSearchBar");
			animating = true;
			GameTaskManager.Instance.AddTask(async () => { while (animating) await Task.Yield(); });
		}

		public void DropdownAnimationFinished()
		{
			IsDropdownOpen = !IsDropdownOpen;
			animating = false;

			if (!IsDropdownOpen)
			{
				searchBar.text = "";
			}
		}
	}

}