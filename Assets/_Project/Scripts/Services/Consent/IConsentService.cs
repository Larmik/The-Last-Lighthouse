using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Services.Consent
{
    public interface IConsentService
    {
        UniTask<bool> GatherConsentAsync(CancellationToken cancellationToken = default);
        bool CanRequestAds { get; }
        void ShowPrivacyOptions();
    }
}
