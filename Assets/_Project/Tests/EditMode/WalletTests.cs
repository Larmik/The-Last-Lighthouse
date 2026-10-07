using System;
using System.Collections.Generic;
using Game.Core;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class WalletTests
    {
        private Wallet wallet;
        private List<(CurrencyType Type, long Balance)> changes;

        [SetUp]
        public void SetUp()
        {
            wallet = new Wallet();
            changes = new List<(CurrencyType Type, long Balance)>();
            wallet.Changed += (type, balance) => changes.Add((type, balance));
        }

        [TestCaseSource(nameof(AllCurrencies))]
        public void Get_NewWallet_ReturnsZero(CurrencyType type) =>
            Assert.That(wallet.Get(type), Is.EqualTo(0));

        [Test]
        public void Add_PositiveAmount_IncreasesBalance()
        {
            wallet.Add(CurrencyType.Light, 180);
            wallet.Add(CurrencyType.Light, 20);

            Assert.That(wallet.Get(CurrencyType.Light), Is.EqualTo(200));
        }

        [Test]
        public void Add_OneCurrency_LeavesOtherCurrenciesUnchanged()
        {
            wallet.Add(CurrencyType.Pearls, 25);

            Assert.That(wallet.Get(CurrencyType.Light), Is.EqualTo(0));
            Assert.That(wallet.Get(CurrencyType.Fragments), Is.EqualTo(0));
            Assert.That(wallet.Get(CurrencyType.Oil), Is.EqualTo(0));
        }

        [Test]
        public void Add_AnyAmount_RaisesChangedWithNewBalance()
        {
            wallet.Add(CurrencyType.Fragments, 10);
            wallet.Add(CurrencyType.Fragments, 15);

            Assert.That(changes, Is.EqualTo(new[] { (CurrencyType.Fragments, 10L), (CurrencyType.Fragments, 25L) }));
        }

        [Test]
        public void Add_NegativeAmount_ThrowsWithoutChange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => wallet.Add(CurrencyType.Light, -1));

            Assert.That(wallet.Get(CurrencyType.Light), Is.EqualTo(0));
            Assert.That(changes, Is.Empty);
        }

        [Test]
        public void CanAfford_AmountUpToBalance_ReturnsTrue()
        {
            wallet.Add(CurrencyType.Light, 80);

            Assert.That(wallet.CanAfford(CurrencyType.Light, 80), Is.True);
            Assert.That(wallet.CanAfford(CurrencyType.Light, 0), Is.True);
        }

        [Test]
        public void CanAfford_AmountAboveBalance_ReturnsFalse()
        {
            wallet.Add(CurrencyType.Light, 79);

            Assert.That(wallet.CanAfford(CurrencyType.Light, 80), Is.False);
        }

        [Test]
        public void TrySpend_SufficientBalance_DeductsAndReturnsTrue()
        {
            wallet.Add(CurrencyType.Light, 200);

            var spent = wallet.TrySpend(CurrencyType.Light, 80);

            Assert.That(spent, Is.True);
            Assert.That(wallet.Get(CurrencyType.Light), Is.EqualTo(120));
        }

        [Test]
        public void TrySpend_ExactBalance_LeavesZero()
        {
            wallet.Add(CurrencyType.Oil, 1);

            Assert.That(wallet.TrySpend(CurrencyType.Oil, 1), Is.True);
            Assert.That(wallet.Get(CurrencyType.Oil), Is.EqualTo(0));
        }

        [Test]
        public void TrySpend_SufficientBalance_RaisesChangedWithNewBalance()
        {
            wallet.Add(CurrencyType.Pearls, 100);
            changes.Clear();

            wallet.TrySpend(CurrencyType.Pearls, 30);

            Assert.That(changes, Is.EqualTo(new[] { (CurrencyType.Pearls, 70L) }));
        }

        [Test]
        public void TrySpend_InsufficientBalance_ReturnsFalseAndKeepsBalance()
        {
            wallet.Add(CurrencyType.Light, 79);

            var spent = wallet.TrySpend(CurrencyType.Light, 80);

            Assert.That(spent, Is.False);
            Assert.That(wallet.Get(CurrencyType.Light), Is.EqualTo(79));
        }

        [Test]
        public void TrySpend_InsufficientBalance_DoesNotRaiseChanged()
        {
            wallet.Add(CurrencyType.Light, 79);
            changes.Clear();

            wallet.TrySpend(CurrencyType.Light, 80);

            Assert.That(changes, Is.Empty);
        }

        [Test]
        public void TrySpend_EmptyWallet_ReturnsFalse() =>
            Assert.That(wallet.TrySpend(CurrencyType.Fragments, 1), Is.False);

        [Test]
        public void TrySpend_NegativeAmount_ThrowsWithoutChange()
        {
            wallet.Add(CurrencyType.Light, 10);
            changes.Clear();

            Assert.Throws<ArgumentOutOfRangeException>(() => wallet.TrySpend(CurrencyType.Light, -5));

            Assert.That(wallet.Get(CurrencyType.Light), Is.EqualTo(10));
            Assert.That(changes, Is.Empty);
        }

        private static IEnumerable<CurrencyType> AllCurrencies() => (CurrencyType[])Enum.GetValues(typeof(CurrencyType));
    }
}
