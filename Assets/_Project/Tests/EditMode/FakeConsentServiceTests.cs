using Game.Services.Consent;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class FakeConsentServiceTests
    {
        [Test]
        public void CanRequestAds_BeforeGathering_IsFalse() =>
            Assert.That(new FakeConsentService().CanRequestAds, Is.False);

        [Test]
        public void GatherConsentAsync_ByDefault_GrantsConsent()
        {
            var service = new FakeConsentService();

            var granted = service.GatherConsentAsync().GetAwaiter().GetResult();

            Assert.That(granted, Is.True);
            Assert.That(service.CanRequestAds, Is.True);
        }

        [Test]
        public void GatherConsentAsync_ConsentRefused_ForbidsAds()
        {
            var service = new FakeConsentService { GrantsConsent = false };

            var granted = service.GatherConsentAsync().GetAwaiter().GetResult();

            Assert.That(granted, Is.False);
            Assert.That(service.CanRequestAds, Is.False);
        }

        [Test]
        public void ShowPrivacyOptions_Called_IsCounted()
        {
            var service = new FakeConsentService();

            service.ShowPrivacyOptions();

            Assert.That(service.PrivacyOptionsShownCount, Is.EqualTo(1));
        }
    }
}
