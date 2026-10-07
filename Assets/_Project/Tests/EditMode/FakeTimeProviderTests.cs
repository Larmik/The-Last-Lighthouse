using System;
using Game.Core.Time;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class FakeTimeProviderTests
    {
        private static readonly DateTime Start = new(2026, 3, 1, 22, 30, 0, DateTimeKind.Utc);

        [Test]
        public void Constructor_UtcTime_ExposesTimeAndOffset()
        {
            var provider = new FakeTimeProvider(Start, TimeSpan.FromHours(2));

            Assert.That(provider.UtcNow, Is.EqualTo(Start));
            Assert.That(provider.UtcNow.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(provider.LocalOffset, Is.EqualTo(TimeSpan.FromHours(2)));
        }

        [Test]
        public void Constructor_NoOffset_DefaultsToZero() =>
            Assert.That(new FakeTimeProvider(Start).LocalOffset, Is.EqualTo(TimeSpan.Zero));

        [Test]
        public void Constructor_UnspecifiedKind_IsTreatedAsUtc()
        {
            var provider = new FakeTimeProvider(new DateTime(2026, 3, 1, 22, 30, 0));

            Assert.That(provider.UtcNow, Is.EqualTo(Start));
            Assert.That(provider.UtcNow.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test]
        public void Constructor_LocalKind_Throws() =>
            Assert.Throws<ArgumentException>(() => new FakeTimeProvider(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Local)));

        [Test]
        public void Advance_Duration_MovesTimeForward()
        {
            var provider = new FakeTimeProvider(Start);

            provider.Advance(TimeSpan.FromMinutes(25));

            Assert.That(provider.UtcNow, Is.EqualTo(Start.AddMinutes(25)));
            Assert.That(provider.UtcNow.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test]
        public void SetUtcNow_EarlierTime_MovesClockBackward()
        {
            var provider = new FakeTimeProvider(Start);

            provider.SetUtcNow(Start.AddHours(-3));

            Assert.That(provider.UtcNow, Is.EqualTo(Start.AddHours(-3)));
        }

        [Test]
        public void LocalOffset_TimeZoneChange_KeepsUtcTime()
        {
            var provider = new FakeTimeProvider(Start, TimeSpan.FromHours(1));

            provider.LocalOffset = TimeSpan.FromHours(-5);

            Assert.That(provider.UtcNow, Is.EqualTo(Start));
            Assert.That(provider.LocalOffset, Is.EqualTo(TimeSpan.FromHours(-5)));
        }
    }
}
