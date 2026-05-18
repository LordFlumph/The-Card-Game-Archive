namespace CardGameArchive
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

	public class MenuGameUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI gameText;
		[SerializeField] MenuGamePopup popup;

        [SerializeField] Button button;

		public void Setup(GameTerms.GameName gameName)
        {
            popup.Setup(gameName);
        }
	} 
}
