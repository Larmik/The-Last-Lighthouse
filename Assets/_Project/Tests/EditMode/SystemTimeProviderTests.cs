using System;
using Game.Core.Time;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class SystemTimeProviderTests
    {
        [Test]
        public void UtcNow_Always_IsUtcKind() =>
            Assert.That(new SystemTimeProvider().UtcNow.Kind, Is.EqualTo(DateTimeKind.Utc));

        [Test]
        public void LocalOffset_Always_IsWithinRealTimeZoneRange()
        {
            var offset = new SystemTimeProvider().LocalOffset;

            Assert.That(offset, Is.InRange(TimeSpan.FromHours(-14), TimeSpan.FromHours(14)));
        }
    }
}
