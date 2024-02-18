using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EasySaveApp.Models
{
    class BackupFile
    {
        public string FileName { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public BackupType Type { get; set; }
        public long FileSize { get; set; }
        public string FileTransferTime { get; set; }

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
            BackupFile backup = new BackupFile(FileName, FileSource, FileTarget, Type);
            backups.Add(backup);
            BackupFile.SaveBackupsToFile();
            return backup;
        }
        public void ExecuteCopy()
        {
            string BackupSaveFolder = Path.Combine(FileTarget, FileName);
            Directory.CreateDirectory(BackupSaveFolder);

            foreach (var filePath in Directory.GetFiles(FileSource))
            {
                var fileName = Path.GetFileName(filePath);
                var targetPath = Path.Combine(BackupSaveFolder, fileName);

                if (Type == BackupType.Full || (Type == BackupType.Differential && File.GetLastWriteTime(filePath) > File.GetLastWriteTime(targetPath)))
                {
                    File.Copy(filePath, targetPath, true);
                    CopiedFiles.Add(filePath);
                }
            }

            foreach (var DirectoryPath in Directory.GetDirectories(FileSource))
            {
                var directoryName = Path.GetFileName(DirectoryPath);

                if (!directoryName.Equals(FileName, StringComparison.OrdinalIgnoreCase))
                {
                    var newDirectoryTarget = Path.Combine(BackupSaveFolder, directoryName);
                    CopyDirectory(DirectoryPath, newDirectoryTarget);
                }
                else
                {
                    CopyDirectory(DirectoryPath, BackupSaveFolder);
                }
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destFile, true);
                CopiedFiles.Add(file);
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var subDirName = Path.GetFileName(subDir);
                var newTargetDir = Path.Combine(targetDir, subDirName);
                CopyDirectory(subDir, newTargetDir);
            }
        }

        public static void SaveBackupsToFile()
        {
            string jsonString = JsonConvert.SerializeObject(backups, Formatting.Indented);
            File.WriteAllText("backups.json", jsonString);
        }

        public static void LoadBackupsFromFile()
        {
            if (File.Exists("backups.json"))
            {
                string jsonString = File.ReadAllText("backups.json");
                backups = JsonConvert.DeserializeObject<List<BackupFile>>(jsonString);
            }
        }
    }
}
