using System;

namespace Game.Core.Time
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
        TimeSpan LocalOffset { get; }
    }
}
