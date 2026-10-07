using Newtonsoft.Json.Linq;

namespace Game.Core.Save
{
    public interface ISaveMigration
    {
        int FromVersion { get; }
        void Apply(JObject save);
    }
}
