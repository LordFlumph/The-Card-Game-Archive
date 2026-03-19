namespace CardGameArchive
{
    using UnityEngine;

    public abstract class BaseGameRules
    {
        public abstract bool CheckWinCondition();
        public virtual int GetScore() => 0;
    }
}