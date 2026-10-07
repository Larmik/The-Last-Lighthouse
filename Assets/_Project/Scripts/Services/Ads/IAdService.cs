using Cysharp.Threading.Tasks;

namespace Game.Services.Ads
{
    public interface IAdService
    {
        bool IsRewardedReady { get; }
        UniTask<bool> ShowRewardedAsync(string placement);
        UniTask ShowInterstitialAsync(string placement);
    }
}
