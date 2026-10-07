using Cysharp.Threading.Tasks;
using Game.Data.Scenes;
using UnityEngine;

namespace Game.Bootstrap
{
    public sealed class BootStartup : MonoBehaviour
    {
        private readonly ISceneLoader sceneLoader = new SceneLoader();

        private void Start() => sceneLoader.LoadAsync(GameScene.Hub, destroyCancellationToken).Forget();
    }
}
