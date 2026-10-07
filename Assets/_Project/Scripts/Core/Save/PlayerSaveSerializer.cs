using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Game.Core.Save
{
    public sealed class PlayerSaveSerializer
    {
        private readonly SaveMigrator migrator;
        private readonly JsonSerializer jsonSerializer;

        public PlayerSaveSerializer() : this(PlayerSave.CurrentVersion, SaveMigrations.All)
        {
        }

        public PlayerSaveSerializer(int currentVersion, IReadOnlyList<ISaveMigration> migrations)
        {
            migrator = new SaveMigrator(currentVersion, migrations);
            jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                Converters = { new StringEnumConverter() }
            });
        }

        public string Serialize(PlayerSave save)
        {
            if (save == null) throw new ArgumentNullException(nameof(save));

            using var writer = new StringWriter(CultureInfo.InvariantCulture);
            jsonSerializer.Serialize(writer, save);
            return writer.ToString();
        }

        public PlayerSave Deserialize(string json)
        {
            var document = Parse(json);
            migrator.Migrate(document);
            try
            {
                return document.ToObject<PlayerSave>(jsonSerializer);
            }
            catch (JsonException exception)
            {
                throw new SaveDataException("Save content does not match the save model.", exception);
            }
        }

        private static JObject Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new SaveDataException("Save is empty.");

            JToken root;
            try
            {
                root = JToken.Parse(json);
            }
            catch (JsonException exception)
            {
                throw new SaveDataException("Save is not valid JSON.", exception);
            }

            return root as JObject ?? throw new SaveDataException("Save root is not a JSON object.");
        }
    }
}
