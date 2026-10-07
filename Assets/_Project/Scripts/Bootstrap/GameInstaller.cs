using Game.Core;
using Game.Core.Time;
using Game.Data.Scenes;
using Game.Services.Ads;
using Game.Services.Analytics;
using Game.Services.Consent;
using Game.Services.Purchases;
using Game.Services.RemoteConfig;
using Game.Services.Save;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameInstaller : IInstaller
    {
        private readonly string saveDirectoryPath;

        public GameInstaller(string saveDirectoryPath) => this.saveDirectoryPath = saveDirectoryPath;

        public void Install(IContainerBuilder builder)
        {
            builder.Register<ITimeProvider, SystemTimeProvider>(Lifetime.Singleton);
            builder.Register<ISaveService, JsonSaveService>(Lifetime.Singleton).WithParameter(saveDirectoryPath);
            builder.Register<IConsentService, FakeConsentService>(Lifetime.Singleton);
            builder.Register<IRemoteConfigService, FakeRemoteConfigService>(Lifetime.Singleton);
            builder.Register<IAnalyticsService, FakeAnalyticsService>(Lifetime.Singleton);
            builder.Register<IAdService, FakeAdService>(Lifetime.Singleton);
            builder.Register<IPurchaseService, FakePurchaseService>(Lifetime.Singleton);
            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);
            builder.Register<Wallet>(Lifetime.Singleton);
        }
    }
}
