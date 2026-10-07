using System;
using System.Collections.Generic;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class IslandState
    {
        public bool BossDefeated;
        public Dictionary<string, int> BuildingLevels = new();
        public int ConsecutiveDefeats;
    }
}
