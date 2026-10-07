using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Services.Consent
{
    public sealed class FakeConsentService : IConsentService
    {
        public bool GrantsConsent { get; set; } = true;
        public bool CanRequestAds { get; private set; }
        public int PrivacyOptionsShownCount { get; private set; }

        public UniTask<bool> GatherConsentAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<bool>(cancellationToken);

            CanRequestAds = GrantsConsent;
            return UniTask.FromResult(CanRequestAds);
        }

        public void ShowPrivacyOptions() => PrivacyOptionsShownCount++;
    }
}
