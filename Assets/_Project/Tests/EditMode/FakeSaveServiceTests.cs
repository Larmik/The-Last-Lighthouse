using System;
using Game.Core;
using Game.Core.Save;
using Game.Services.Save;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class FakeSaveServiceTests
    {
        private FakeSaveService service;

        [SetUp]
        public void SetUp() => service = new FakeSaveService();

        [Test]
        public void Load_NothingSaved_ReturnsNewSave()
        {
            var save = service.Load();

            Assert.That(save.Version, Is.EqualTo(PlayerSave.CurrentVersion));
            Assert.That(save.RunsCompleted, Is.Zero);
            Assert.That(save.HighestIsland, Is.EqualTo(1));
        }

        [Test]
        public void SaveThenLoad_Save_RoundTrips()
        {
            var save = new PlayerSave { RunsCompleted = 4 };
            save.Balances[CurrencyType.Pearls] = 40;

            service.Save(save);
            var restored = service.Load();

            Assert.That(restored.RunsCompleted, Is.EqualTo(4));
            Assert.That(restored.Balances[CurrencyType.Pearls], Is.EqualTo(40));
        }

        [Test]
        public void Load_SavedInstanceMutatedAfterSave_ReturnsSavedState()
        {
            var save = new PlayerSave { RunsCompleted = 1 };
            service.Save(save);

            save.RunsCompleted = 2;

            Assert.That(service.Load().RunsCompleted, Is.EqualTo(1));
        }

        [Test]
        public void Load_Twice_ReturnsIndependentCopies()
        {
            service.Save(new PlayerSave { RunsCompleted = 1 });

            Assert.That(service.Load(), Is.Not.SameAs(service.Load()));
        }

        [Test]
        public void Save_Null_ThrowsAndKeepsPreviousSave()
        {
            service.Save(new PlayerSave { RunsCompleted = 3 });

            Assert.Throws<ArgumentNullException>(() => service.Save(null));
            Assert.That(service.Load().RunsCompleted, Is.EqualTo(3));
        }
    }
}
