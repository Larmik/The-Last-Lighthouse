using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Services.Purchases
{
    public interface IPurchaseService
    {
        UniTask InitializeAsync(CancellationToken cancellationToken = default);
        UniTask<bool> PurchaseAsync(string productId, CancellationToken cancellationToken = default);
        UniTask RestoreAsync(CancellationToken cancellationToken = default);
        bool Owns(string productId);
    }
}
