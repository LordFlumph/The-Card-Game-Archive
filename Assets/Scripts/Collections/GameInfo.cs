namespace CardGameArchive
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameInfo", menuName = "Card Game Archive/Game Info")]
    public class GameInfo : ScriptableObject
    {
        [SerializeField] GameTerms.GameName gameName;
        [SerializeField] List<GameTerms.GameTag> tags;
        [SerializeField][TextArea] string aboutText;
    }
}