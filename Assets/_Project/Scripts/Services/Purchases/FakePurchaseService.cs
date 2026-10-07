using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.Services.Purchases
{
    public sealed class FakePurchaseService : IPurchaseService
    {
        private readonly HashSet<string> accountProducts = new();
        private readonly HashSet<string> ownedProducts = new();

        public bool IsInitialized { get; private set; }
        public bool CompletesPurchases { get; set; } = true;

        public void AddPreviousPurchase(string productId)
        {
            RequireProductId(productId);
            if (ProductIds.IsConsumable(productId))
                throw new ArgumentException($"Consumable product '{productId}' cannot be restored.", nameof(productId));

            accountProducts.Add(productId);
        }

        public UniTask InitializeAsync()
        {
            IsInitialized = true;
            return UniTask.CompletedTask;
        }

        public UniTask<bool> PurchaseAsync(string productId)
        {
            RequireProductId(productId);
            if (!IsInitialized || !CompletesPurchases || Owns(productId)) return UniTask.FromResult(false);

            if (!ProductIds.IsConsumable(productId))
            {
                accountProducts.Add(productId);
                ownedProducts.Add(productId);
            }

            return UniTask.FromResult(true);
        }

        public UniTask RestoreAsync()
        {
            if (IsInitialized) ownedProducts.UnionWith(accountProducts);
            return UniTask.CompletedTask;
        }

        public bool Owns(string productId) => productId != null && ownedProducts.Contains(productId);

        private static void RequireProductId(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException("Product id is required.", nameof(productId));
        }
    }
}
