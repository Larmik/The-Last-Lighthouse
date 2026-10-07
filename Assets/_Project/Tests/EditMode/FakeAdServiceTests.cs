using System;
using Game.Services.Ads;
using Game.Services.Consent;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class FakeAdServiceTests
    {
        private const string InterstitialPlacement = "run_end";

        private FakeConsentService consentService;
        private FakeAdService service;

        [SetUp]
        public void SetUp()
        {
            consentService = new FakeConsentService();
            service = new FakeAdService(consentService);
        }

        [Test]
        public void Constructor_MissingConsentService_Throws() =>
            Assert.Throws<ArgumentNullException>(() => new FakeAdService(null));

        [Test]
        public void IsRewardedReady_BeforeConsent_IsFalse() =>
            Assert.That(service.IsRewardedReady, Is.False);

        [Test]
        public void ShowRewardedAsync_BeforeConsent_ReturnsFalseWithoutShowing()
        {
            var rewarded = service.ShowRewardedAsync(AdPlacements.Revive).GetAwaiter().GetResult();

            Assert.That(rewarded, Is.False);
            Assert.That(service.RewardedShownCount, Is.Zero);
        }

        [TestCase(AdPlacements.Revive)]
        [TestCase(AdPlacements.DoubleRewards)]
        [TestCase(AdPlacements.ExtraLampOil)]
        [TestCase(AdPlacements.LightCacheX2)]
        [TestCase(AdPlacements.Reroll)]
        public void ShowRewardedAsync_AfterConsent_Rewards(string placement)
        {
            GrantConsent();

            var rewarded = service.ShowRewardedAsync(placement).GetAwaiter().GetResult();

            Assert.That(rewarded, Is.True);
            Assert.That(service.RewardedShownCount, Is.EqualTo(1));
        }

        [Test]
        public void ShowRewardedAsync_AdClosedEarly_ReturnsFalse()
        {
            GrantConsent();
            service.CompletesRewarded = false;

            var rewarded = service.ShowRewardedAsync(AdPlacements.Reroll).GetAwaiter().GetResult();

            Assert.That(rewarded, Is.False);
            Assert.That(service.RewardedShownCount, Is.EqualTo(1));
        }

        [Test]
        public void ShowRewardedAsync_NoAdAvailable_ReturnsFalseWithoutShowing()
        {
            GrantConsent();
            service.RewardedAvailable = false;

            var rewarded = service.ShowRewardedAsync(AdPlacements.Revive).GetAwaiter().GetResult();

            Assert.That(service.IsRewardedReady, Is.False);
            Assert.That(rewarded, Is.False);
            Assert.That(service.RewardedShownCount, Is.Zero);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("unknown_placement")]
        public void ShowRewardedAsync_UnknownPlacement_Throws(string placement) =>
            Assert.Throws<ArgumentException>(() => service.ShowRewardedAsync(placement));

        [Test]
        public void ShowInterstitialAsync_BeforeConsent_ShowsNothing()
        {
            service.ShowInterstitialAsync(InterstitialPlacement).GetAwaiter().GetResult();

            Assert.That(service.InterstitialShownCount, Is.Zero);
        }

        [Test]
        public void ShowInterstitialAsync_AfterConsent_IsCounted()
        {
            GrantConsent();

            service.ShowInterstitialAsync(InterstitialPlacement).GetAwaiter().GetResult();

            Assert.That(service.InterstitialShownCount, Is.EqualTo(1));
        }

        [TestCase(null)]
        [TestCase("")]
        public void ShowInterstitialAsync_MissingPlacement_Throws(string placement) =>
            Assert.Throws<ArgumentException>(() => service.ShowInterstitialAsync(placement));

        private void GrantConsent() => consentService.GatherConsentAsync().GetAwaiter().GetResult();
    }
}
