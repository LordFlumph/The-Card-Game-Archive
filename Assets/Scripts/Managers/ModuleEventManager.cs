namespace CardGameArchive.Behaviours
{
    using System;
    using UnityEngine;

    public class ModuleEventManager : MonoBehaviour
    {
        public static ModuleEventManager Instance {get; private set;}

        public Action<Deck> OnDeckTapped;
        public Action<GameMove> OnUndo;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        void OnDestroy()
        {
            
        }
    }
}

