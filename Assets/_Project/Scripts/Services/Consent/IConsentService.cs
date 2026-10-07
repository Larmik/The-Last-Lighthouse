using Cysharp.Threading.Tasks;

namespace Game.Services.Consent
{
    public interface IConsentService
    {
        UniTask<bool> GatherConsentAsync();
        bool CanRequestAds { get; }
        void ShowPrivacyOptions();
    }
}
