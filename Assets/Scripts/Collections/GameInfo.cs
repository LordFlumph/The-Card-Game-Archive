namespace CardGameArchive
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameInfo", menuName = "Card Game Archive/Game Info")]
    public class GameInfo : ScriptableObject
    {
        [field: SerializeField] public GameTerms.GameName Name { get; private set; }
		[System.Serializable] public struct GameVariantInfo 
        {
            public GameTerms.GameVariant Variant;
            [SerializeField] string displayName;
            public string DisplayName { get { return string.IsNullOrEmpty(displayName) ? Variant.ToString() : displayName; } }
            public string Description; 
            public Sprite Icon;
        }
		[field: SerializeField] public List<GameVariantInfo> Variants { get; private set; }
        [SerializeField] string displayName;
        public string DisplayName => string.IsNullOrEmpty(displayName) ? Name.ToString() : displayName;
		[field: SerializeField] public List<GameTerms.GameTag> Tags { get; private set; }
		[field: SerializeField] public Sprite Icon { get; private set; }
		[field: SerializeField][field: TextArea(3, 20)] public string AboutText { get; private set; }
		[field: SerializeField][field: TextArea(3, 20)] public string GuideText { get; private set; }
        
	}
}