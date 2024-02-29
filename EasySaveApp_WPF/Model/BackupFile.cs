﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using EasySaveApp_WPF.ViewModel;
using Newtonsoft.Json;

namespace EasySaveApp_WPF.Models
{
    public class BackupFile : IBase , INotifyPropertyChanged
    {
        public static bool canBeExecuted = true;
        private static bool IsInExecution = false;

        // Properties for backup file details
        public string FileName { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public BackupType Type { get; set; }
        public long FileSize { get; set; }
        public string FileTransferTime { get; set; }
        public bool IsPaused { get; set; }
        public bool IsStoped { get; set; }

        public VMSettings Settings { get; set; }

        public List<string> CopiedFiles { get; set; }

        public static List<BackupFile> backups = new List<BackupFile>();

        public bool Executed { get; set; }

        public static List<BackupFile> Backups
        {
            get { return backups; }
            set { backups = value; }
        }

        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BackupFile(string FileName, string FileSource, string FileTarget, BackupType Type, bool IsPaused)
        {
            this.FileName = FileName;
            this.FileSource = FileSource;
            this.FileTarget = FileTarget;
            this.Type = Type;
            this.IsPaused = IsPaused;
            CopiedFiles = new List<string>();
        }

        // Method to monitor processes
        public static void MonitorProcess()
        {
            // Start the process monitoring thread
            Process[] processes = Process.GetProcessesByName("CalculatorApp");
            if (processes.Length > 0)   // check if a software of the list is running
            {
                canBeExecuted = false;
                if (IsInExecution)
                {
                    // Pause
                    Thread.Sleep(1000);
                }
            }
            else
            {
                canBeExecuted = true;
            }
        }

        // Method to create a backup
        public static BackupFile CreateBackup(string FileName, string FileSource, string FileTarget, BackupType Type, bool IsPaused)
        {
            if (BackupHandler.BackupHandlerInstance == null)
                BackupHandler.BackupHandlerInstance = new BackupHandler();
            BackupFile backup = new BackupFile(FileName, FileSource, FileTarget, Type, IsPaused);
            BackupHandler.BackupHandlerInstance.UpdateBackup(backup);
            return backup;
        }

        // Method to execute the backup copy process
        public void ExecuteCopy(BackupFile backup)
        {
            string BackupSaveFolder = Path.Combine(FileTarget, FileName);
            Directory.CreateDirectory(BackupSaveFolder);

            int totalFiles = Directory.GetFiles(FileSource, "*", SearchOption.AllDirectories).Length;
            int currentFile = 0;

            foreach (var filePath in Directory.GetFiles(FileSource))
            {
                
                var fileName = Path.GetFileName(filePath);
                var targetPath = Path.Combine(BackupSaveFolder, fileName);

                if (Type == BackupType.Full || (Type == BackupType.Differential && File.GetLastWriteTime(filePath) > File.GetLastWriteTime(targetPath)))
                {
                    while (!canBeExecuted || backup.IsPaused) 
                    {
                        Thread.Sleep(1000);
                    }
                    File.Copy(filePath, targetPath, true);
                    CopiedFiles.Add(filePath);

                    currentFile++;
                    Progress = (int)(((double)currentFile / totalFiles) * 100);
                    OnPropertyChanged(nameof(Progress));
                }
            }

            foreach (var DirectoryPath in Directory.GetDirectories(FileSource))
            {
                while (!canBeExecuted || backup.IsPaused) // Attendre si la sauvegarde est en pause
                {
                    Thread.Sleep(1000);
                }
                var directoryName = Path.GetFileName(DirectoryPath);
                var newDirectoryTarget = Path.Combine(BackupSaveFolder, directoryName);
                CopyDirectory(DirectoryPath, newDirectoryTarget);
            }
        }

        // Method to recursively copy directories
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

        public void Dispose()
        {
            // Release unmanaged resources if necessary
        }

        // Méthode pour mettre la sauvegarde en pause
        public void Pause()
        {
            IsPaused = true;
        }

        // Méthode pour reprendre la sauvegarde
        public void Resume()
        {
            IsPaused = false;
        }

        // Méthode pour arrêter complètement la sauvegarde
        public void Stop()
        {
            IsStoped = true;
        }
    }

    // Class for handling backups
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

        // Method to update a backup
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

        // Method to save backups to JSON
        public void SaveBackupsToJson()
        {
            string json = JsonConvert.SerializeObject(_saveBackups, Formatting.Indented);
            File.WriteAllText("backups.json", json);
        }

        // Method to load backups from JSON
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

        // Method to delete a backup
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

    // Enumeration for backup types
    public enum BackupType
    {
        Full,
        Differential
    }
}
