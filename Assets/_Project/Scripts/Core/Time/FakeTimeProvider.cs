using System;

namespace Game.Core.Time
{
    public sealed class FakeTimeProvider : ITimeProvider
    {
        private DateTime utcNow;

        public FakeTimeProvider(DateTime utcNow, TimeSpan localOffset = default)
        {
            SetUtcNow(utcNow);
            LocalOffset = localOffset;
        }

        public DateTime UtcNow => utcNow;
        public TimeSpan LocalOffset { get; set; }

        public void SetUtcNow(DateTime value)
        {
            if (value.Kind == DateTimeKind.Local)
                throw new ArgumentException("Fake time must be expressed in UTC.", nameof(value));

            utcNow = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public void Advance(TimeSpan duration) => utcNow = utcNow.Add(duration);
    }
}
