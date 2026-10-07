using System;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class DailyState
    {
        public int CycleDay;
        public long LastClaimTimestamp;
        public int Streak;
        public long ForgivenDayTimestamp;
    }
}
