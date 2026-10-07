using System;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class RunInProgress
    {
        public string IslandId;
        public int Wave;
        public long StartedTimestamp;
        public bool LampOilCharged;
    }
}
