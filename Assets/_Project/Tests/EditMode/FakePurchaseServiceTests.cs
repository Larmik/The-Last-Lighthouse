using System;
using System.Threading;
using Game.Services.Purchases;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class FakePurchaseServiceTests
    {
        private FakePurchaseService service;

        [SetUp]
        public void SetUp() => service = new FakePurchaseService();

        [Test]
        public void PurchaseAsync_BeforeInitialization_Fails()
        {
            var purchased = service.PurchaseAsync(ProductIds.RemoveAds).GetAwaiter().GetResult();

            Assert.That(purchased, Is.False);
            Assert.That(service.Owns(ProductIds.RemoveAds), Is.False);
        }

        [Test]
        public void PurchaseAsync_NonConsumable_IsOwned()
        {
            Initialize();

            var purchased = service.PurchaseAsync(ProductIds.RemoveAds).GetAwaiter().GetResult();

            Assert.That(purchased, Is.True);
            Assert.That(service.Owns(ProductIds.RemoveAds), Is.True);
        }

        [Test]
        public void PurchaseAsync_NonConsumableAlreadyOwned_FailsWithoutDoubleCredit()
        {
            Initialize();
            service.PurchaseAsync(ProductIds.StarterPack).GetAwaiter().GetResult();

            var purchasedAgain = service.PurchaseAsync(ProductIds.StarterPack).GetAwaiter().GetResult();

            Assert.That(purchasedAgain, Is.False);
        }

        [Test]
        public void PurchaseAsync_Consumable_SucceedsRepeatedlyWithoutOwnership()
        {
            Initialize();

            var first = service.PurchaseAsync(ProductIds.PearlsS).GetAwaiter().GetResult();
            var second = service.PurchaseAsync(ProductIds.PearlsS).GetAwaiter().GetResult();

            Assert.That(first, Is.True);
            Assert.That(second, Is.True);
            Assert.That(service.Owns(ProductIds.PearlsS), Is.False);
        }

        [Test]
        public void PurchaseAsync_Cancelled_FailsWithoutOwnership()
        {
            Initialize();
            service.CompletesPurchases = false;

            var purchased = service.PurchaseAsync(ProductIds.RemoveAds).GetAwaiter().GetResult();

            Assert.That(purchased, Is.False);
            Assert.That(service.Owns(ProductIds.RemoveAds), Is.False);
        }

        [Test]
        public void InitializeAsync_Cancelled_ThrowsWithoutInitializing()
        {
            Assert.Throws<OperationCanceledException>(() => service.InitializeAsync(new CancellationToken(true)).GetAwaiter().GetResult());
            Assert.That(service.IsInitialized, Is.False);
        }

        [Test]
        public void PurchaseAsync_Cancelled_ThrowsWithoutOwnership()
        {
            Initialize();

            Assert.Throws<OperationCanceledException>(() =>
                service.PurchaseAsync(ProductIds.RemoveAds, new CancellationToken(true)).GetAwaiter().GetResult());
            Assert.That(service.Owns(ProductIds.RemoveAds), Is.False);
        }

        [Test]
        public void RestoreAsync_Cancelled_ThrowsWithoutRestoring()
        {
            service.AddPreviousPurchase(ProductIds.RemoveAds);
            Initialize();

            Assert.Throws<OperationCanceledException>(() => service.RestoreAsync(new CancellationToken(true)).GetAwaiter().GetResult());
            Assert.That(service.Owns(ProductIds.RemoveAds), Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        public void PurchaseAsync_MissingProductId_Throws(string productId) =>
            Assert.Throws<ArgumentException>(() => service.PurchaseAsync(productId));

        [Test]
        public void RestoreAsync_PreviousPurchase_IsOwnedAgain()
        {
            service.AddPreviousPurchase(ProductIds.RemoveAds);
            Initialize();

            Assert.That(service.Owns(ProductIds.RemoveAds), Is.False);

            service.RestoreAsync().GetAwaiter().GetResult();

            Assert.That(service.Owns(ProductIds.RemoveAds), Is.True);
        }

        [Test]
        public void RestoreAsync_BeforeInitialization_RestoresNothing()
        {
            service.AddPreviousPurchase(ProductIds.RemoveAds);

            service.RestoreAsync().GetAwaiter().GetResult();

            Assert.That(service.Owns(ProductIds.RemoveAds), Is.False);
        }

        [Test]
        public void AddPreviousPurchase_Consumable_Throws() =>
            Assert.Throws<ArgumentException>(() => service.AddPreviousPurchase(ProductIds.PearlsXxl));

        [Test]
        public void Owns_NullProductId_IsFalse() =>
            Assert.That(service.Owns(null), Is.False);

        [TestCase(ProductIds.PearlsS, true)]
        [TestCase(ProductIds.PearlsM, true)]
        [TestCase(ProductIds.PearlsL, true)]
        [TestCase(ProductIds.PearlsXl, true)]
        [TestCase(ProductIds.PearlsXxl, true)]
        [TestCase(ProductIds.RemoveAds, false)]
        [TestCase(ProductIds.StarterPack, false)]
        [TestCase("season_pass_s1", false)]
        public void IsConsumable_Product_MatchesBrief(string productId, bool expected) =>
            Assert.That(ProductIds.IsConsumable(productId), Is.EqualTo(expected));

        private void Initialize() => service.InitializeAsync().GetAwaiter().GetResult();
    }
}
