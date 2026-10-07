namespace Game.Services.Analytics
{
    public interface IAnalyticsService
    {
        void Log(string eventName, params (string Key, object Value)[] parameters);
    }
}
