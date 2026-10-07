using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Bootstrap;
using Game.Data.Scenes;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VContainer;

namespace Game.Tests.PlayMode
{
    public sealed class SceneFlowTests
    {
        private const int BootToHubTimeoutMilliseconds = 10000;

        [TearDown]
        public void TearDown()
        {
            foreach (var scope in Object.FindObjectsByType<GameLifetimeScope>(FindObjectsSortMode.None))
            {
                Object.Destroy(scope.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator BootScene_OnStart_ReachesHubThenRunThenHub() => UniTask.ToCoroutine(async () =>
        {
            await SceneManager.LoadSceneAsync(nameof(GameScene.Boot), LoadSceneMode.Single).ToUniTask();
            await WaitForActiveSceneAsync(GameScene.Hub);
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(nameof(GameScene.Hub)));

            var sceneLoader = FindSingleGameLifetimeScope().Container.Resolve<ISceneLoader>();

            await sceneLoader.LoadAsync(GameScene.Run, CancellationToken.None);
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(nameof(GameScene.Run)));

            await sceneLoader.LoadAsync(GameScene.Hub, CancellationToken.None);
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(nameof(GameScene.Hub)));
            Assert.That(FindSingleGameLifetimeScope().Container.Resolve<ISceneLoader>(), Is.SameAs(sceneLoader));
        });

        private static GameLifetimeScope FindSingleGameLifetimeScope()
        {
            var scopes = Object.FindObjectsByType<GameLifetimeScope>(FindObjectsSortMode.None);
            Assert.That(scopes, Has.Length.EqualTo(1));
            return scopes[0];
        }

        private static async UniTask WaitForActiveSceneAsync(GameScene scene)
        {
            using var timeout = new CancellationTokenSource(BootToHubTimeoutMilliseconds);
            await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == scene.ToString(), cancellationToken: timeout.Token);
        }
    }
}
