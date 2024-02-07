using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            if (backups.Count > NumberMaxOfSave)
                throw new Exception("Maximum number of Backup reached");
            BackupFile backup = Type switch
            {
                BackupType.Full => new FullBackupFile(FileName, FileSource, FileTarget),
                BackupType.Differential => new DifferentialBackupFile(FileName, FileSource, FileTarget)
            };
            backups.Add(backup);
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
