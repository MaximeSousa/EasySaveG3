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
        public BackupType Type { get; set; }

        public static List<BackupFile> backups = new List<BackupFile>() { null };
        public List<string> CopiedFiles { get; set; }
        public const int NumberMaxOfSave = 5;

        public BackupFile(string FileName, string FileSource, string FileTarget, BackupType Type)
        {
            this.FileName = FileName;
            this.FileSource = FileSource;
            this.FileTarget = FileTarget;
            this.Type = Type;
            CopiedFiles = new List<string>();
        }
        public static BackupFile CreateBackup(string FileName, string FileSource, string FileTarget, BackupType Type)
        {
            LoadBackupsFromFile();
            if (backups.Count >= NumberMaxOfSave)
                throw new Exception("Maximum number of Backup reached");
            BackupFile backup = Type switch
            {
                BackupType.Full => new FullBackupFile(FileName, FileSource, FileTarget,Type),
                BackupType.Differential => new DifferentialBackupFile(FileName, FileSource, FileTarget,Type),
                _ => throw new ArgumentException("Invalid backup type", nameof(Type))
            };
            backups.Add(backup);
            BackupFile.SaveBackupsToFile();
            return backup;
        }
        public void ExecuteCopy()
        {
            if (Type == BackupType.Full)
            {
                foreach (var filePath in Directory.GetFiles(FileSource))
                {
                    var fileName = Path.GetFileName(filePath);
                    var targetPath = Path.Combine(FileTarget, fileName);
                    File.Copy(filePath, targetPath, true);
                    CopiedFiles.Add(filePath);
                }
            }
            else if (Type == BackupType.Differential)
            {
                foreach (var filePath in Directory.GetFiles(FileSource))
                {
                    var fileName = Path.GetFileName(filePath);
                    var targetPath = Path.Combine(FileTarget, fileName);

                    if (File.GetLastWriteTime(filePath) > File.GetLastWriteTime(targetPath))
                    {
                        File.Copy(filePath, targetPath, true);
                        CopiedFiles.Add(filePath);
                    }
                }
            }

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

                var subDirectoryBackup = new BackupFile(FileName, directoryPath, newDirectoryTarget, Type);
                subDirectoryBackup.ExecuteCopy();
            }
        }

        public static void SaveBackupsToFile()
        {
            string jsonString = JsonSerializer.Serialize(backups);
            File.WriteAllText("backups.json", jsonString);
        }

        public static void LoadBackupsFromFile()
        {
            if (File.Exists("backups.json"))
            {
                string jsonString = File.ReadAllText("backups.json");
                backups = JsonSerializer.Deserialize<List<BackupFile>>(jsonString);
            }
        }
    }
    class FullBackupFile : BackupFile
    {
        public FullBackupFile(string FileName, string FileSource, string FileTarget,BackupType Type) :base(FileName, FileSource, FileTarget, Type)
        {
        }
    }
    class DifferentialBackupFile : BackupFile
    {
        public DifferentialBackupFile(string FileName, string FileSource, string FileTarget, BackupType Type) : base(FileName, FileSource, FileTarget, Type)
        {
        }
    }
}
