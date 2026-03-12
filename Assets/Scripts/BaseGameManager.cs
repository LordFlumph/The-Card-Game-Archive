using UnityEngine;

public abstract class BaseGameManager : MonoBehaviour
{
    public static BaseGameManager Instance {get; private set;}

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public abstract void OnDeckClicked(Deck deck);
    public abstract void OnCardClicked(Card card);
    public abstract void OnCardDropped(Card card);
}
