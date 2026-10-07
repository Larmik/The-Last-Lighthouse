using Cysharp.Threading.Tasks;

namespace Game.Services.Consent
{
    public sealed class FakeConsentService : IConsentService
    {
        public bool GrantsConsent { get; set; } = true;
        public bool CanRequestAds { get; private set; }
        public int PrivacyOptionsShownCount { get; private set; }

        public UniTask<bool> GatherConsentAsync()
        {
            CanRequestAds = GrantsConsent;
            return UniTask.FromResult(CanRequestAds);
        }

        public void ShowPrivacyOptions() => PrivacyOptionsShownCount++;
    }
}
