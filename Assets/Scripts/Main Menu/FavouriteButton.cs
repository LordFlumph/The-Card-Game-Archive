namespace CardGameArchive.MainMenu
{
	using UnityEngine;

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

		public void ToggleFavourite()
		{
			if (favouriteAnimating)
				return;

			favouriteAnimator.Play(isFavourite ? "Unfavourite" : "Favourite");
			favouriteAnimating = true;
			parentVariant.OnFavouriteClick();
		}

		public void FavouriteAnimationFinished()
		{
			favouriteAnimating = false;
			isFavourite = !isFavourite;
		}
	}

}