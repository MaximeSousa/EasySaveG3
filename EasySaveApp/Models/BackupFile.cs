using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace EasySaveApp.Models
{
    class BackupFile
    {
        public string FileName { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public BackupType FileType { get; set; }

        public static List<BackupFile> backups = new List<BackupFile>();
        public const int NumberMaxOfSave = 5;

        public BackupFile(string FileName, string FileSource, string FileTarget, BackupType FileType)
        {
            this.FileName = FileName;
            this.FileSource = FileSource;
            this.FileTarget = FileTarget;
            this.FileType = FileType;
        }
        public static BackupFile CreateBackup(string FileName, string FileSource, string FileTarget, BackupType Type)
        {
            BackupFile.LoadBackupsFromFile();
            if (backups.Count > NumberMaxOfSave)
                throw new Exception("Maximum number of Backup reached");
            BackupFile backup = Type switch
            {
                BackupType.Full => new FullBackupFile(FileName, FileSource, FileTarget),
                BackupType.Differential => new DifferentialBackupFile(FileName, FileSource, FileTarget)
            };
            backups.Add(backup);
            BackupFile.SaveBackupsToFile();
            return backup;
        }
        public void ExecuteCopy()
        {

            foreach (var FilePath in Directory.GetFiles(FileSource))
            {
                var FileName = Path.GetFileName(FilePath);
                File.Copy(FilePath, Path.Combine(FileTarget, FileName), true);
            }

            foreach (var directoryPath in Directory.GetDirectories(FileSource))
            {
                var directoryName = Path.GetFileName(directoryPath);
                var newDirectoryTarget = Path.Combine(FileTarget, directoryName);
                Directory.CreateDirectory(newDirectoryTarget);

                var subDirectoryBackup = new BackupFile(FileName, directoryPath, newDirectoryTarget, FileType);
                subDirectoryBackup.ExecuteCopy();
            }
        }

        public static void SaveBackupsToFile()
        {
            string jsonString = JsonSerializer.Serialize(backups);
            File.WriteAllText(@"C:\Users\maxim\source\repos\EasySaveApp\EasySaveApp\bin\Debug\net5.0\backups.json", jsonString);
        }

        public static void LoadBackupsFromFile()
        {
            if (File.Exists(@"C:\Users\maxim\source\repos\EasySaveApp\EasySaveApp\bin\Debug\net5.0\backups.json"))
            {
                string jsonString = File.ReadAllText(@"C:\Users\maxim\source\repos\EasySaveApp\EasySaveApp\bin\Debug\net5.0\backups.json");
                backups = JsonSerializer.Deserialize<List<BackupFile>>(jsonString);
            }
        }
    }
    class FullBackupFile : BackupFile
    {
        public FullBackupFile(string FileName, string FileSource, string FileTarget) :base(FileName, FileSource, FileTarget, BackupType.Full)
        {
        }
    }
    class DifferentialBackupFile : BackupFile
    {
        public DifferentialBackupFile(string FileName, string FileSource, string FileTarget) : base(FileName, FileSource, FileTarget, BackupType.Differential)
        {
        }
    }
}
