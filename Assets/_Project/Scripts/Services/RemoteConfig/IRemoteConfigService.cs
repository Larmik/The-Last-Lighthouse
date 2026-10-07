namespace Game.Services.RemoteConfig
{
    public interface IRemoteConfigService
    {
        T Get<T>(string key, T fallback);
    }
}
