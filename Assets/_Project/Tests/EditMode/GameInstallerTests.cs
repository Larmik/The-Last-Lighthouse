using System;
using System.IO;
using Game.Bootstrap;
using Game.Core;
using Game.Core.Time;
using Game.Data.Scenes;
using Game.Services.Ads;
using Game.Services.Analytics;
using Game.Services.Consent;
using Game.Services.Purchases;
using Game.Services.RemoteConfig;
using Game.Services.Save;
using NUnit.Framework;
using VContainer;

namespace Game.Tests.EditMode
{
    public sealed class GameInstallerTests
    {
        private static readonly Type[] RegisteredTypes =
        {
            typeof(ITimeProvider),
            typeof(ISaveService),
            typeof(IConsentService),
            typeof(IRemoteConfigService),
            typeof(IAnalyticsService),
            typeof(IAdService),
            typeof(IPurchaseService),
            typeof(ISceneLoader),
            typeof(Wallet)
        };

        private IObjectResolver container;

        [SetUp]
        public void SetUp()
        {
            var builder = new ContainerBuilder();
            new GameInstaller(Path.Combine(Path.GetTempPath(), "GameInstallerTests")).Install(builder);
            builder.Register<BootFlow>(Lifetime.Singleton);
            container = builder.Build();
        }

        [TearDown]
        public void TearDown() => container.Dispose();

        [TestCaseSource(nameof(RegisteredTypes))]
        public void Resolve_RegisteredType_ReturnsSameInstance(Type registeredType)
        {
            var first = container.Resolve(registeredType);

            Assert.That(first, Is.Not.Null);
            Assert.That(container.Resolve(registeredType), Is.SameAs(first));
        }

        [Test]
        public void Resolve_BootFlow_GetsAllDependenciesFromContainer() =>
            Assert.That(container.Resolve<BootFlow>(), Is.Not.Null);

        [Test]
        public void Resolve_SaveService_WritesToDisk() =>
            Assert.That(container.Resolve<ISaveService>(), Is.TypeOf<JsonSaveService>());

        [Test]
        public void Resolve_TimeProvider_UsesSystemClock() =>
            Assert.That(container.Resolve<ITimeProvider>(), Is.TypeOf<SystemTimeProvider>());
    }
}
