using System;
using System.Collections.Generic;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class SeasonState
    {
        public string SeasonId;
        public int Xp;
        public HashSet<int> ClaimedFreeTiers = new();
        public HashSet<int> ClaimedPremiumTiers = new();
    }
}
