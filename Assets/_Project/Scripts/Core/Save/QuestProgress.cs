using System;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class QuestProgress
    {
        public string QuestId;
        public int Progress;
        public bool Claimed;
    }
}
