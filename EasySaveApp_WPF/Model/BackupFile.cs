using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;

namespace EasySaveApp_WPF.Models
{
    public class BackupFile : IBase
    {
        public string FileName { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public BackupType Type { get; set; }
        public long FileSize { get; set; }
        public string FileTransferTime { get; set; }

        public List<string> CopiedFiles { get; set; }

        public static List<BackupFile> backups = new List<BackupFile>();



        public bool Executed { get; set; }


        public static List<BackupFile> Backups
        {
            get { return backups; }
            set { backups = value; }
        }

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
            //LoadBackupsFromFile();
            if (BackupHandler.BackupHandlerInstance == null)
                BackupHandler.BackupHandlerInstance = new BackupHandler();
            BackupFile backup = new BackupFile(FileName, FileSource, FileTarget, Type);
            BackupHandler.BackupHandlerInstance.UpdateBackup(backup);
            //backups.Add(backup);
            //SaveBackupsToFile();
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
                var newDirectoryTarget = Path.Combine(BackupSaveFolder, directoryName);
                CopyDirectory(DirectoryPath, newDirectoryTarget);
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



        //public static void SaveBackupsToFile()
        //{

        //    string jsonString = JsonSerializer.Serialize(backups);
        //    File.WriteAllText("backups.json", jsonString);
        //}

        //public static List<BackupFile> LoadBackupsFromFile()
        //{
        //    if (File.Exists("backups.json"))
        //    {
        //        string jsonString = File.ReadAllText("backups.json");
        //        backups = JsonSerializer.Deserialize<List<BackupFile>>(jsonString);
        //    }
        //    return backups;
        //}



        public void Dispose()
        {
            // Libérer les ressources non managées si nécessaire
        }
    }

    public class BackupHandler
    {
        private static BackupHandler _backupHandlerInstance;
        public static BackupHandler BackupHandlerInstance
        {
            get
            {
                if (_backupHandlerInstance == null)
                    _backupHandlerInstance = new BackupHandler();
                return _backupHandlerInstance;
            }
            set { _backupHandlerInstance = value; }
        }

        public List<BackupFile> _saveBackups;

        public BackupHandler()
        {
            if (File.Exists("backups.json"))
            {
                LoadBackupsFromJson();
            }
            else
            {
                _saveBackups = new List<BackupFile>();
            }
        }

        public void UpdateBackup(BackupFile backup)
        {

            int index = _saveBackups.FindIndex(b => b.FileName == backup.FileName && b.FileSource == backup.FileSource && b.FileTarget == backup.FileTarget);
          
            if (index != -1)
            { 
                // Mettre à jour la sauvegarde existante
                _saveBackups[index] = backup;
                SaveBackupsToJson();
            }
            else
            {
                // Ajouter la sauvegarde si elle n'existe pas déjà
                _saveBackups.Add(backup);
                SaveBackupsToJson();
            }
        }

        public void SaveBackupsToJson()
        {
            string json = JsonConvert.SerializeObject(_saveBackups, Formatting.Indented);
            File.WriteAllText("backups.json", json);
        }

        public List<BackupFile> LoadBackupsFromJson()
        {
            if (File.Exists("backups.json"))
            {
                string json = File.ReadAllText("backups.json");
                _saveBackups = JsonConvert.DeserializeObject<List<BackupFile>>(json);
                return _saveBackups;
            }
            return null;
        }

        public void DeleteBackup(BackupFile backup)
        {
            int index = _saveBackups.FindIndex(b => b.FileName == backup.FileName && b.FileSource == backup.FileSource && b.FileTarget == backup.FileTarget);
            if (index != -1)
            {
                _saveBackups.RemoveAt(index);
                SaveBackupsToJson();
            }
        }
    }

    public enum BackupType
    {
        Full,
        Differential
    }
}