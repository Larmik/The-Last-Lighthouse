using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Bootstrap;
using Game.Core.Save;
using Game.Data.Scenes;
using Game.Services.Save;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Tests.EditMode
{
    public sealed class BootFlowTests
    {
        private RecordingSceneLoader sceneLoader;

        [SetUp]
        public void SetUp() => sceneLoader = new RecordingSceneLoader();

        [Test]
        public void StartAsync_ReadableSave_LoadsHubOnly()
        {
            var bootFlow = new BootFlow(new FakeSaveService(), sceneLoader);

            bootFlow.StartAsync().GetAwaiter().GetResult();

            Assert.That(sceneLoader.LoadedScenes, Is.EqualTo(new[] { GameScene.Hub }));
        }

        [Test]
        public void StartAsync_UnreadableSave_StaysOnBootWithoutWriting()
        {
            var saveService = new UnreadableSaveService();
            var bootFlow = new BootFlow(saveService, sceneLoader);
            LogAssert.Expect(LogType.Error, new Regex("Save cannot be loaded"));

            bootFlow.StartAsync().GetAwaiter().GetResult();

            Assert.That(sceneLoader.LoadedScenes, Is.Empty);
            Assert.That(saveService.SaveCount, Is.Zero);
        }

        private sealed class RecordingSceneLoader : ISceneLoader
        {
            private readonly List<GameScene> loadedScenes = new();

            public IReadOnlyList<GameScene> LoadedScenes => loadedScenes;

            public UniTask LoadAsync(GameScene scene, CancellationToken cancellationToken)
            {
                loadedScenes.Add(scene);
                return UniTask.CompletedTask;
            }
        }

        private sealed class UnreadableSaveService : ISaveService
        {
            public int SaveCount { get; private set; }

            public PlayerSave Load() => throw new SaveDataException("Neither the save nor its backup can be read.");

            public void Save(PlayerSave data) => SaveCount++;
        }
    }
}
