using System;
using System.Collections.Generic;
using Game.Core.Save;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class SaveMigratorTests
    {
        private List<int> appliedFromVersions;

        [SetUp]
        public void SetUp() => appliedFromVersions = new List<int>();

        [Test]
        public void Migrate_V1ToV2_AppliesMigrationAndSetsVersion()
        {
            var migrator = new SaveMigrator(2, new[] { new RecordingMigration(1, appliedFromVersions) });
            var save = JObject.Parse("{\"Version\":1}");

            migrator.Migrate(save);

            Assert.That(appliedFromVersions, Is.EqualTo(new[] { 1 }));
            Assert.That(save.Value<int>(SaveMigrator.VersionField), Is.EqualTo(2));
        }

        [Test]
        public void Migrate_V1ToV3_AppliesMigrationsInOrder()
        {
            var migrations = new[] { new RecordingMigration(2, appliedFromVersions), new RecordingMigration(1, appliedFromVersions) };
            var migrator = new SaveMigrator(3, migrations);
            var save = JObject.Parse("{\"Version\":1}");

            migrator.Migrate(save);

            Assert.That(appliedFromVersions, Is.EqualTo(new[] { 1, 2 }));
            Assert.That(save.Value<int>(SaveMigrator.VersionField), Is.EqualTo(3));
        }

        [Test]
        public void Migrate_IntermediateVersion_AppliesOnlyRemainingMigrations()
        {
            var migrations = new[] { new RecordingMigration(1, appliedFromVersions), new RecordingMigration(2, appliedFromVersions) };
            var migrator = new SaveMigrator(3, migrations);

            migrator.Migrate(JObject.Parse("{\"Version\":2}"));

            Assert.That(appliedFromVersions, Is.EqualTo(new[] { 2 }));
        }

        [Test]
        public void Migrate_CurrentVersion_AppliesNothing()
        {
            var migrator = new SaveMigrator(2, new[] { new RecordingMigration(1, appliedFromVersions) });
            var save = JObject.Parse("{\"Version\":2}");

            migrator.Migrate(save);

            Assert.That(appliedFromVersions, Is.Empty);
            Assert.That(save.Value<int>(SaveMigrator.VersionField), Is.EqualTo(2));
        }

        [Test]
        public void Migrate_MissingVersion_TreatedAsFirstVersion()
        {
            var migrator = new SaveMigrator(2, new[] { new RecordingMigration(1, appliedFromVersions) });
            var save = new JObject();

            migrator.Migrate(save);

            Assert.That(appliedFromVersions, Is.EqualTo(new[] { 1 }));
            Assert.That(save.Value<int>(SaveMigrator.VersionField), Is.EqualTo(2));
        }

        [Test]
        public void Migrate_NewerVersion_ThrowsSaveDataException()
        {
            var migrator = new SaveMigrator(1, Array.Empty<ISaveMigration>());

            Assert.Throws<SaveDataException>(() => migrator.Migrate(JObject.Parse("{\"Version\":2}")));
        }

        [TestCase("{\"Version\":\"two\"}")]
        [TestCase("{\"Version\":1.5}")]
        [TestCase("{\"Version\":0}")]
        public void Migrate_InvalidVersion_ThrowsSaveDataException(string json)
        {
            var migrator = new SaveMigrator(2, new[] { new RecordingMigration(1, appliedFromVersions) });

            Assert.Throws<SaveDataException>(() => migrator.Migrate(JObject.Parse(json)));
            Assert.That(appliedFromVersions, Is.Empty);
        }

        [Test]
        public void Constructor_MissingIntermediateMigration_Throws() =>
            Assert.Throws<ArgumentException>(() => new SaveMigrator(3, new[] { new RecordingMigration(1, appliedFromVersions) }));

        [Test]
        public void Constructor_DuplicateMigration_Throws() =>
            Assert.Throws<ArgumentException>(() =>
                new SaveMigrator(2, new[] { new RecordingMigration(1, appliedFromVersions), new RecordingMigration(1, appliedFromVersions) }));

        [TestCase(0)]
        [TestCase(2)]
        public void Constructor_MigrationOutsideVersionRange_Throws(int fromVersion) =>
            Assert.Throws<ArgumentException>(() => new SaveMigrator(2, new[] { new RecordingMigration(fromVersion, appliedFromVersions) }));

        [Test]
        public void Constructor_ProductionMigrations_CoverCurrentVersion() =>
            Assert.DoesNotThrow(() => new SaveMigrator(PlayerSave.CurrentVersion, SaveMigrations.All));

        private sealed class RecordingMigration : ISaveMigration
        {
            private readonly List<int> appliedFromVersions;

            public RecordingMigration(int fromVersion, List<int> appliedFromVersions)
            {
                FromVersion = fromVersion;
                this.appliedFromVersions = appliedFromVersions;
            }

            public int FromVersion { get; }

            public void Apply(JObject save) => appliedFromVersions.Add(FromVersion);
        }
    }
}
