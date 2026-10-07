using System;
using System.IO;
using System.Text;
using Game.Core.Save;

namespace Game.Services.Save
{
    public sealed class JsonSaveService : ISaveService
    {
        internal const string MainFileName = "player_save.json";
        internal const string BackupFileName = "player_save.backup.json";
        internal const string TemporaryFileName = "player_save.tmp";
        internal const string UnreadableFileName = "player_save.unreadable.json";

        private static readonly Encoding FileEncoding = new UTF8Encoding(false);

        private readonly PlayerSaveSerializer serializer = new();
        private readonly string directoryPath;
        private readonly string mainFilePath;
        private readonly string backupFilePath;
        private readonly string temporaryFilePath;
        private readonly string unreadableFilePath;

        public JsonSaveService(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Save directory path is required.", nameof(directoryPath));

            this.directoryPath = directoryPath;
            mainFilePath = Path.Combine(directoryPath, MainFileName);
            backupFilePath = Path.Combine(directoryPath, BackupFileName);
            temporaryFilePath = Path.Combine(directoryPath, TemporaryFileName);
            unreadableFilePath = Path.Combine(directoryPath, UnreadableFileName);
        }

        public PlayerSave Load()
        {
            var mainExists = File.Exists(mainFilePath);
            var backupExists = File.Exists(backupFilePath);
            if (!mainExists && !backupExists) return new PlayerSave();

            if (mainExists && TryRead(mainFilePath, out var mainSave)) return mainSave;
            if (backupExists && TryRead(backupFilePath, out var backupSave)) return backupSave;

            throw new SaveDataException("Neither the save nor its backup can be read.");
        }

        public void Save(PlayerSave data)
        {
            var json = serializer.Serialize(data);
            serializer.Deserialize(json);

            Directory.CreateDirectory(directoryPath);
            WriteDurably(temporaryFilePath, json);
            MoveCurrentMainAside();
            File.Move(temporaryFilePath, mainFilePath);
        }

        private void MoveCurrentMainAside()
        {
            if (!File.Exists(mainFilePath)) return;

            var destination = TryRead(mainFilePath, out _) ? backupFilePath : unreadableFilePath;
            File.Delete(destination);
            File.Move(mainFilePath, destination);
        }

        private bool TryRead(string path, out PlayerSave save)
        {
            try
            {
                save = serializer.Deserialize(File.ReadAllText(path, FileEncoding));
                return true;
            }
            catch (Exception exception) when (exception is SaveDataException or IOException or UnauthorizedAccessException)
            {
                save = null;
                return false;
            }
        }

        private static void WriteDurably(string path, string content)
        {
            var bytes = FileEncoding.GetBytes(content);
            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush(true);
        }
    }
}
