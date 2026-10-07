using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Data.Scenes;
using UnityEngine.SceneManagement;

namespace Game.Bootstrap
{
    public sealed class SceneLoader : ISceneLoader
    {
        public UniTask LoadAsync(GameScene scene, CancellationToken cancellationToken) =>
            SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single).ToUniTask(cancellationToken: cancellationToken);
    }
}
