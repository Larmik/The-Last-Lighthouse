using System;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class EnergyState
    {
        public int Amount;
        public long LastRegenTimestamp;
        public int BonusAboveCap;
    }
}
