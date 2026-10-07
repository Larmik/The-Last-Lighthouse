using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Services.Ads
{
    public interface IAdService
    {
        bool IsRewardedReady { get; }
        UniTask<bool> ShowRewardedAsync(string placement, CancellationToken cancellationToken = default);
        UniTask ShowInterstitialAsync(string placement, CancellationToken cancellationToken = default);
    }
}
