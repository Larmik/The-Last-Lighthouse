using System;
using Cysharp.Threading.Tasks;
using Game.Services.Consent;

namespace Game.Services.Ads
{
    public sealed class FakeAdService : IAdService
    {
        private readonly IConsentService consentService;

        public FakeAdService(IConsentService consentService) =>
            this.consentService = consentService ?? throw new ArgumentNullException(nameof(consentService));

        public bool RewardedAvailable { get; set; } = true;
        public bool CompletesRewarded { get; set; } = true;
        public int RewardedShownCount { get; private set; }
        public int InterstitialShownCount { get; private set; }

        public bool IsRewardedReady => consentService.CanRequestAds && RewardedAvailable;

        public UniTask<bool> ShowRewardedAsync(string placement)
        {
            if (!AdPlacements.IsRewarded(placement))
                throw new ArgumentException($"Unknown rewarded placement '{placement}'.", nameof(placement));

            if (!IsRewardedReady) return UniTask.FromResult(false);

            RewardedShownCount++;
            return UniTask.FromResult(CompletesRewarded);
        }

        public UniTask ShowInterstitialAsync(string placement)
        {
            if (string.IsNullOrEmpty(placement))
                throw new ArgumentException("Interstitial placement is required.", nameof(placement));

            if (consentService.CanRequestAds) InterstitialShownCount++;
            return UniTask.CompletedTask;
        }
    }
}
