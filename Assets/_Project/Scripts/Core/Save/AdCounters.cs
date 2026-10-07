using System;
using System.Collections.Generic;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class AdCounters
    {
        public long CountersDayTimestamp;
        public Dictionary<string, int> DailyRewardedCounts = new();
        public long LastInterstitialTimestamp;
        public int RunsSinceLastInterstitial;
    }
}
