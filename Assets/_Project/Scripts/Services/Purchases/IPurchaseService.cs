using Cysharp.Threading.Tasks;

namespace Game.Services.Purchases
{
    public interface IPurchaseService
    {
        UniTask InitializeAsync();
        UniTask<bool> PurchaseAsync(string productId);
        UniTask RestoreAsync();
        bool Owns(string productId);
    }
}
