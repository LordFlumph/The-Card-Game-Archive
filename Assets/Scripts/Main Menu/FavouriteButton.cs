namespace CardGameArchive.MainMenu
{
	using System.Threading.Tasks;
	using UnityEngine;
	using static UnityEditor.UIElements.ToolbarMenu;

	public class FavouriteButton : MonoBehaviour
	{
		Animator favouriteAnimator;
		bool isFavourite = false;
		bool favouriteAnimating = false;

		[SerializeField] GameVariantButton parentVariant;

		private void Awake()
		{
			favouriteAnimator = GetComponent<Animator>();
		}

		void Start()
		{
			if (GameDataManager.Instance.IsFavourite(parentVariant.Variant))
				ToggleFavourite();
		}

		public void ToggleFavourite()
		{
			if (favouriteAnimating)
				return;

			favouriteAnimator.Play(isFavourite ? "Unfavourite" : "Favourite");
			favouriteAnimating = true;
			GameTaskManager.Instance.AddTask(async () => { while (favouriteAnimating) await Task.Yield(); });
			GameDataManager.Instance.SetFavourite(parentVariant.Variant, !isFavourite);
		}

		public void FavouriteAnimationFinished()
		{
			favouriteAnimating = false;
			isFavourite = !isFavourite;
		}
	}

}