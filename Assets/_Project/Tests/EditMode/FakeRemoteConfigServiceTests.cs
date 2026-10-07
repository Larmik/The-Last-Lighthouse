using System;
using Game.Services.RemoteConfig;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class FakeRemoteConfigServiceTests
    {
        private const string LampOilMaxKey = "lamp_oil_max";

        private FakeRemoteConfigService service;

        [SetUp]
        public void SetUp() => service = new FakeRemoteConfigService();

        [Test]
        public void Get_NoOverride_ReturnsFallback() =>
            Assert.That(service.Get(LampOilMaxKey, 5), Is.EqualTo(5));

        [Test]
        public void Get_Override_ReturnsOverride()
        {
            service.SetOverride(LampOilMaxKey, 7);

            Assert.That(service.Get(LampOilMaxKey, 5), Is.EqualTo(7));
        }

        [Test]
        public void Get_OverrideOfAnotherType_ReturnsFallback()
        {
            service.SetOverride(LampOilMaxKey, "seven");

            Assert.That(service.Get(LampOilMaxKey, 5), Is.EqualTo(5));
        }

        [Test]
        public void Get_ClearedOverride_ReturnsFallback()
        {
            service.SetOverride(LampOilMaxKey, 7);

            service.ClearOverride(LampOilMaxKey);

            Assert.That(service.Get(LampOilMaxKey, 5), Is.EqualTo(5));
        }

        [TestCase(null)]
        [TestCase("")]
        public void Get_MissingKey_Throws(string key) =>
            Assert.Throws<ArgumentException>(() => service.Get(key, 5));

        [TestCase(null)]
        [TestCase("")]
        public void SetOverride_MissingKey_Throws(string key) =>
            Assert.Throws<ArgumentException>(() => service.SetOverride(key, 5));
    }
}
