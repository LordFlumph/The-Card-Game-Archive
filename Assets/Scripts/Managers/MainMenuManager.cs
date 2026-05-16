namespace CardGameArchive
{
    using UnityEngine;
    using UnityEngine.UI;

    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] LayoutElement headerDeadzoneElement;
        [SerializeField] LayoutElement footerDeadzoneElement;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
			if (Screen.safeArea.yMax != Screen.height)
				headerDeadzoneElement.minHeight = Screen.height - Screen.safeArea.yMax + 25;
			if (Screen.safeArea.yMin > 0)
				footerDeadzoneElement.minHeight = Screen.safeArea.yMin + 25;
		}

        // Update is called once per frame
        void Update()
        {
		}
    }

}