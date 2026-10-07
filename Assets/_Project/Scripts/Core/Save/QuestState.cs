using System;
using System.Collections.Generic;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class QuestState
    {
        public long AssignedTimestamp;
        public List<QuestProgress> Active = new();
        public bool CompletionBonusClaimed;
    }
}
