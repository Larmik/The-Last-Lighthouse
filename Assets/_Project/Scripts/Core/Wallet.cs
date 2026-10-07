using System;
using System.Collections.Generic;

namespace Game.Core
{
    public sealed class Wallet
    {
        private readonly Dictionary<CurrencyType, long> balances = new Dictionary<CurrencyType, long>();

        public event Action<CurrencyType, long> Changed;

        public long Get(CurrencyType type) => balances.TryGetValue(type, out var value) ? value : 0;

        public bool CanAfford(CurrencyType type, long amount) => Get(type) >= amount;

        public void Add(CurrencyType type, long amount)
        {
            EnsureNotNegative(amount);
            SetBalance(type, Get(type) + amount);
        }

        public bool TrySpend(CurrencyType type, long amount)
        {
            EnsureNotNegative(amount);
            if (!CanAfford(type, amount)) return false;
            SetBalance(type, Get(type) - amount);
            return true;
        }

        private void SetBalance(CurrencyType type, long balance)
        {
            balances[type] = balance;
            Changed?.Invoke(type, balance);
        }

        private static void EnsureNotNegative(long amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must not be negative.");
        }
    }
}
