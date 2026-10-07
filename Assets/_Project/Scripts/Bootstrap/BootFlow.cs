using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Save;
using Game.Data.Scenes;
using Game.Services.Save;
using UnityEngine;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class BootFlow : IAsyncStartable
    {
        private readonly ISaveService saveService;
        private readonly ISceneLoader sceneLoader;

        public BootFlow(ISaveService saveService, ISceneLoader sceneLoader)
        {
            this.saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            this.sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            if (!TryLoadSave()) return;

            await sceneLoader.LoadAsync(GameScene.Hub, cancellation);
        }

        private bool TryLoadSave()
        {
            try
            {
                saveService.Load();
                return true;
            }
            catch (SaveDataException exception)
            {
                Debug.LogError($"[BootFlow] Save cannot be loaded, staying on Boot without overwriting it: {exception.Message}");
                return false;
            }
        }
    }
}
