namespace CardGameArchive
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameInfo", menuName = "Card Game Archive/Game Info")]
    public class GameInfo : ScriptableObject
    {
        [field: SerializeField] public GameTerms.GameName Name { get; private set; }
        [field: SerializeField] public List<GameTerms.GameVariant> Variants { get; private set; }
        [SerializeField] string displayName;
        public string DisplayName => string.IsNullOrEmpty(displayName) ? Name.ToString() : displayName;
		[field: SerializeField] public List<GameTerms.GameTag> Tags { get; private set; }
        [field: SerializeField][field: TextArea(3, 20)] public string AboutText { get; private set; }
    }
}