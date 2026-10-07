using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Data.Scenes
{
    public interface ISceneLoader
    {
        UniTask LoadAsync(GameScene scene, CancellationToken cancellationToken);
    }
}
