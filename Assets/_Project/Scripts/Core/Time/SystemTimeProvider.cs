using System;

namespace Game.Core.Time
{
    public sealed class SystemTimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public TimeSpan LocalOffset => TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
    }
}
