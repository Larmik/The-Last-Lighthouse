using Game.Core.Save;

namespace Game.Services.Save
{
    public sealed class FakeSaveService : ISaveService
    {
        private readonly PlayerSaveSerializer serializer = new();
        private string storedJson;

        public PlayerSave Load() => storedJson == null ? new PlayerSave() : serializer.Deserialize(storedJson);

        public void Save(PlayerSave data)
        {
            var json = serializer.Serialize(data);
            serializer.Deserialize(json);
            storedJson = json;
        }
    }
}
