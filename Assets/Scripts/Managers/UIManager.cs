using CardGameArchive;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] CanvasGroup winScreenGroup;

	private void Awake()
	{
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        BaseGameManager.Instance.RestartGame();
	}

    public void ShowWinScreen()
    {
        winScreenGroup.FadeIn(0.2f);
    }
}
