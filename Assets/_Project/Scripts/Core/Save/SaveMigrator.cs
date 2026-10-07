using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Game.Core.Save
{
    public sealed class SaveMigrator
    {
        public const string VersionField = "Version";
        public const int FirstVersion = 1;

        private readonly int currentVersion;
        private readonly Dictionary<int, ISaveMigration> migrationsByFromVersion = new();

        public SaveMigrator(int currentVersion, IReadOnlyList<ISaveMigration> migrations)
        {
            if (currentVersion < FirstVersion)
                throw new ArgumentOutOfRangeException(nameof(currentVersion), currentVersion, "Current version must be at least the first version.");
            if (migrations == null) throw new ArgumentNullException(nameof(migrations));

            this.currentVersion = currentVersion;
            foreach (var migration in migrations) Register(migration);
            EnsureEveryVersionHasMigration();
        }

        public void Migrate(JObject save)
        {
            if (save == null) throw new ArgumentNullException(nameof(save));

            var version = ReadVersion(save);
            if (version > currentVersion)
                throw new SaveDataException($"Save version {version} is newer than supported version {currentVersion}.");

            while (version < currentVersion)
            {
                migrationsByFromVersion[version].Apply(save);
                version++;
                save[VersionField] = version;
            }
        }

        private void Register(ISaveMigration migration)
        {
            if (migration == null) throw new ArgumentException("Migration list contains a null entry.", nameof(migration));
            if (migration.FromVersion < FirstVersion || migration.FromVersion >= currentVersion)
                throw new ArgumentException($"Migration from version {migration.FromVersion} is outside 1..{currentVersion - 1}.", nameof(migration));
            if (!migrationsByFromVersion.TryAdd(migration.FromVersion, migration))
                throw new ArgumentException($"Duplicate migration from version {migration.FromVersion}.", nameof(migration));
        }

        private void EnsureEveryVersionHasMigration()
        {
            for (var version = FirstVersion; version < currentVersion; version++)
            {
                if (!migrationsByFromVersion.ContainsKey(version))
                    throw new ArgumentException($"Missing migration from version {version} to {version + 1}.");
            }
        }

        private static int ReadVersion(JObject save)
        {
            var token = save[VersionField];
            if (token == null || token.Type == JTokenType.Null) return FirstVersion;
            if (token.Type != JTokenType.Integer) throw new SaveDataException("Save version is not an integer.");

            var version = token.Value<long>();
            if (version < FirstVersion || version > int.MaxValue) throw new SaveDataException($"Save version {version} is invalid.");
            return (int)version;
        }
    }
}
