using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Bootstrap;
using Game.Data.Scenes;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public sealed class SceneFlowTests
    {
        private const int BootToHubTimeoutMilliseconds = 10000;

        [UnityTest]
        public IEnumerator SceneLoader_FromBoot_ReachesHubThenRunThenHub() => UniTask.ToCoroutine(async () =>
        {
            var sceneLoader = new SceneLoader();

            await sceneLoader.LoadAsync(GameScene.Boot, CancellationToken.None);
            await WaitForActiveSceneAsync(GameScene.Hub);
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(nameof(GameScene.Hub)));

            await sceneLoader.LoadAsync(GameScene.Run, CancellationToken.None);
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(nameof(GameScene.Run)));

            await sceneLoader.LoadAsync(GameScene.Hub, CancellationToken.None);
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(nameof(GameScene.Hub)));
        });

        private static async UniTask WaitForActiveSceneAsync(GameScene scene)
        {
            using var timeout = new CancellationTokenSource(BootToHubTimeoutMilliseconds);
            await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == scene.ToString(), cancellationToken: timeout.Token);
        }
    }
}
