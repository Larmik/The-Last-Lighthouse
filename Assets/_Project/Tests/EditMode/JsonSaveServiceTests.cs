using System;
using System.IO;
using Game.Core;
using Game.Core.Save;
using Game.Services.Save;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class JsonSaveServiceTests
    {
        private const string CorruptedContent = "{\"Version\":1,\"RunsCompl";

        private string directoryPath;
        private JsonSaveService service;

        private string MainFilePath => Path.Combine(directoryPath, JsonSaveService.MainFileName);
        private string BackupFilePath => Path.Combine(directoryPath, JsonSaveService.BackupFileName);
        private string TemporaryFilePath => Path.Combine(directoryPath, JsonSaveService.TemporaryFileName);
        private string UnreadableFilePath => Path.Combine(directoryPath, JsonSaveService.UnreadableFileName);

        [SetUp]
        public void SetUp()
        {
            directoryPath = Path.Combine(Path.GetTempPath(), "GameJsonSaveServiceTests", Guid.NewGuid().ToString("N"));
            service = new JsonSaveService(directoryPath);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_MissingDirectoryPath_Throws(string path) =>
            Assert.Throws<ArgumentException>(() => new JsonSaveService(path));

        [Test]
        public void Load_NoFile_ReturnsNewSaveWithoutWriting()
        {
            var save = service.Load();

            Assert.That(save.Version, Is.EqualTo(PlayerSave.CurrentVersion));
            Assert.That(save.RunsCompleted, Is.Zero);
            Assert.That(save.HighestIsland, Is.EqualTo(1));
            Assert.That(Directory.Exists(directoryPath), Is.False);
        }

        [Test]
        public void SaveThenLoad_MissingDirectory_CreatesDirectoryAndRoundTrips()
        {
            service.Save(CreateSave(7));

            var restored = new JsonSaveService(directoryPath).Load();

            Assert.That(restored.RunsCompleted, Is.EqualTo(7));
            Assert.That(restored.Balances[CurrencyType.Pearls], Is.EqualTo(70));
            Assert.That(restored.UnlockedKeepers, Is.EquivalentTo(new[] { "orrin" }));
        }

        [Test]
        public void Save_Completed_LeavesNoTemporaryFile()
        {
            service.Save(CreateSave(1));
            service.Save(CreateSave(2));

            Assert.That(File.Exists(TemporaryFilePath), Is.False);
            Assert.That(Directory.GetFiles(directoryPath), Is.EquivalentTo(new[] { MainFilePath, BackupFilePath }));
        }

        [Test]
        public void Save_FirstTime_CreatesNoBackup()
        {
            service.Save(CreateSave(1));

            Assert.That(File.Exists(MainFilePath), Is.True);
            Assert.That(File.Exists(BackupFilePath), Is.False);
        }

        [Test]
        public void Save_Twice_KeepsPreviousSaveAsBackup()
        {
            service.Save(CreateSave(1));
            service.Save(CreateSave(2));

            Assert.That(service.Load().RunsCompleted, Is.EqualTo(2));
            Assert.That(ReadSaveFile(BackupFilePath).RunsCompleted, Is.EqualTo(1));
        }

        [Test]
        public void Save_StaleTemporaryFile_IsReplaced()
        {
            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(TemporaryFilePath, CorruptedContent);

            service.Save(CreateSave(3));

            Assert.That(File.Exists(TemporaryFilePath), Is.False);
            Assert.That(service.Load().RunsCompleted, Is.EqualTo(3));
        }

        [Test]
        public void Load_CorruptedMain_RecoversFromBackup()
        {
            service.Save(CreateSave(1));
            service.Save(CreateSave(2));
            File.WriteAllText(MainFilePath, CorruptedContent);

            var restored = service.Load();

            Assert.That(restored.RunsCompleted, Is.EqualTo(1));
            Assert.That(File.ReadAllText(MainFilePath), Is.EqualTo(CorruptedContent));
        }

        [Test]
        public void Load_MainNewerThanApp_RecoversFromBackup()
        {
            service.Save(CreateSave(1));
            service.Save(CreateSave(2));
            File.WriteAllText(MainFilePath, $"{{\"Version\":{PlayerSave.CurrentVersion + 1}}}");

            Assert.That(service.Load().RunsCompleted, Is.EqualTo(1));
        }

        [Test]
        public void Load_MainMissingAfterInterruptedSave_RecoversFromBackup()
        {
            service.Save(CreateSave(1));
            service.Save(CreateSave(2));
            File.Delete(MainFilePath);

            Assert.That(service.Load().RunsCompleted, Is.EqualTo(1));
        }

        [Test]
        public void Load_MainAndBackupCorrupted_ThrowsAndKeepsFiles()
        {
            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(MainFilePath, CorruptedContent);
            File.WriteAllText(BackupFilePath, "not json");

            Assert.Throws<SaveDataException>(() => service.Load());
            Assert.That(File.ReadAllText(MainFilePath), Is.EqualTo(CorruptedContent));
            Assert.That(File.ReadAllText(BackupFilePath), Is.EqualTo("not json"));
        }

        [Test]
        public void Load_OnlyCorruptedBackup_Throws()
        {
            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(BackupFilePath, CorruptedContent);

            Assert.Throws<SaveDataException>(() => service.Load());
        }

        [Test]
        public void Save_AfterRecoveryFromCorruptedMain_KeepsValidBackupAndSetsCorruptedMainAside()
        {
            service.Save(CreateSave(1));
            service.Save(CreateSave(2));
            File.WriteAllText(MainFilePath, CorruptedContent);
            var recovered = service.Load();
            recovered.RunsCompleted = 5;

            service.Save(recovered);

            Assert.That(service.Load().RunsCompleted, Is.EqualTo(5));
            Assert.That(ReadSaveFile(BackupFilePath).RunsCompleted, Is.EqualTo(1));
            Assert.That(File.ReadAllText(UnreadableFilePath), Is.EqualTo(CorruptedContent));
        }

        [Test]
        public void Save_UnreadableData_ThrowsAndKeepsLastValidSave()
        {
            service.Save(CreateSave(1));
            var unreadable = CreateSave(2);
            unreadable.Version = PlayerSave.CurrentVersion + 1;

            Assert.Throws<SaveDataException>(() => service.Save(unreadable));
            Assert.That(service.Load().RunsCompleted, Is.EqualTo(1));
            Assert.That(Directory.GetFiles(directoryPath), Is.EquivalentTo(new[] { MainFilePath }));
        }

        [Test]
        public void Save_Null_ThrowsAndWritesNothing()
        {
            Assert.Throws<ArgumentNullException>(() => service.Save(null));
            Assert.That(Directory.Exists(directoryPath), Is.False);
        }

        private static PlayerSave CreateSave(int runsCompleted)
        {
            var save = new PlayerSave { RunsCompleted = runsCompleted };
            save.Balances[CurrencyType.Pearls] = runsCompleted * 10L;
            save.UnlockedKeepers.Add("orrin");
            return save;
        }

        private static PlayerSave ReadSaveFile(string path) => new PlayerSaveSerializer().Deserialize(File.ReadAllText(path));
    }
}
