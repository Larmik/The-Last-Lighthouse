using System;
using System.Collections.Generic;

namespace Game.Services.RemoteConfig
{
    public sealed class FakeRemoteConfigService : IRemoteConfigService
    {
        private readonly Dictionary<string, object> overrides = new();

        public void SetOverride(string key, object value)
        {
            RequireKey(key);
            overrides[key] = value;
        }

        public void ClearOverride(string key)
        {
            RequireKey(key);
            overrides.Remove(key);
        }

        public T Get<T>(string key, T fallback)
        {
            RequireKey(key);
            return overrides.TryGetValue(key, out var value) && value is T typedValue ? typedValue : fallback;
        }

        private static void RequireKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Remote Config key is required.", nameof(key));
        }
    }
}
