using Game.Core.Save;

namespace Game.Services.Save
{
    public interface ISaveService
    {
        PlayerSave Load();
        void Save(PlayerSave data);
    }
}
