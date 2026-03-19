namespace CardGameArchive
{
    using System.Collections.Generic;
    using UnityEngine;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject cardPrefab;
        private List<GameObject> allCards;

        [SerializeField] private GameObject deckObj;

        public void DrawCard(Card card)
        {

        }
    }
}
