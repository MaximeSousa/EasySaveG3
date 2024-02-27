﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EasySaveApp_WPF.Models;
using System.Diagnostics;

namespace EasySaveApp_WPF.ViewModel
{
    public class VMExecuteBackup : VMBaseViewModel
    {
        private ObservableCollection<BackupFile> _backups;
        public ObservableCollection<BackupFile> Backups
        {
            get { return _backups; }
            set
            {
                _backups = value;
                OnPropertyChanged(nameof(Backups));
            }
        }

        private ObservableCollection<BackupFile> _selectedBackups;
        public ObservableCollection<BackupFile> SelectedBackups
        {
            get { return _selectedBackups; }
            set
            {
                _selectedBackups = value;
                OnPropertyChanged(nameof(SelectedBackups));
            }
        }

        private bool _isChange;
        public bool IsChange
        {
            get { return _isChange; }
            set
            {
                _isChange = value;
                OnPropertyChanged(nameof(IsChange));
            }
        }

        public ICommand ChangeBackupCommand { get; private set; }
        public ICommand DeleteBackupCommand { get; private set; }
        public ICommand ExecuteBackupCommand { get; private set; }
        public ICommand VisibleBackupCommand { get; private set; }
        public ICommand PauseBackupCommand { get; private set; }
        public ICommand ContinueBackupCommand { get; private set; }
        public ICommand StopBackupCommand { get; private set; }

        public string BackupName { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public BackupType Type { get; set; }

        public string OutputFormat { get; set; } = "json";

        public VMExecuteBackup()
        {
            LoadBackups();
            SelectedBackups = new ObservableCollection<BackupFile>();
            ChangeBackupCommand = new RelayCommand(ChangeBackup);
            DeleteBackupCommand = new RelayCommand(DeleteBackup);
            ExecuteBackupCommand = new RelayCommand(ExecuteBackup);
            VisibleBackupCommand = new RelayCommand(Visible);
            PauseBackupCommand = new RelayCommand(PauseBackup);
            ContinueBackupCommand = new RelayCommand(ContinueBackup);
            StopBackupCommand = new RelayCommand(StopBackup);
        }

        private void LoadBackups()
        {
            try
            {
                BackupHandler backupHandler = new BackupHandler();
                var loadedBackups = backupHandler.LoadBackupsFromJson();
                if (loadedBackups != null)
                {
                    Backups = new ObservableCollection<BackupFile>(loadedBackups);
                }
                else
                {
                    Backups = new ObservableCollection<BackupFile>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading backups: {ex.Message}");
            }
        }

        private void Visible(object parameter)
        {
            IsChange = true;
        }

        private void ChangeBackup(object parameter)
        {
            if (SelectedBackups != null && SelectedBackups.Count == 1)
            {
                var backup = SelectedBackups[0];

                try
                {
                    BackupFile originalBackup = new BackupFile(backup.FileName, backup.FileSource, backup.FileTarget, backup.Type);
                    if (!string.IsNullOrEmpty(BackupName) && BackupName != backup.FileName)
                    {
                        backup.FileName = BackupName;
                    }

                    if (!string.IsNullOrEmpty(Source) && Source != backup.FileSource)
                    {
                        backup.FileSource = Source;
                    }

                    if (!string.IsNullOrEmpty(Destination) && Destination != backup.FileTarget)
                    {
                        backup.FileTarget = Destination;
                    }

                    if (Type != backup.Type)
                    {
                        backup.Type = Type;
                    }

                    BackupHandler.BackupHandlerInstance.UpdateBackup(originalBackup);

                    BackupHandler.BackupHandlerInstance.SaveBackupsToJson();

                    MessageBox.Show("Backup modification successful.");
                    LoadBackups();
                    IsChange = false;
                }
                finally
                {
                    backup.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Please select a backup to modify.");
            }
        }

        private void DeleteBackup(object parameter)
        {
            if (SelectedBackups != null && SelectedBackups.Count > 0)
            {
                try
                {
                    foreach (var backup in SelectedBackups)
                    {
                        string BackupFolder = Path.Combine(backup.FileTarget, backup.FileName);

                        if (Directory.Exists(BackupFolder))
                        {
                            Directory.Delete(BackupFolder, true);
                        }
                        BackupHandler.BackupHandlerInstance.DeleteBackup(backup);
                        CreateLog(backup.FileName, backup.FileSource, backup.FileTarget, 0, "", "Delete", OutputFormat);
                    }
                    BackupHandler.BackupHandlerInstance.SaveBackupsToJson();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting backup directories: {ex.Message}");
                    return;
                }
                LoadBackups();
                MessageBox.Show("Selected backups deletion successful.");
            }
            else
            {
                MessageBox.Show("Please select a backup to delete.");
            }
        }

        private void ExecuteBackup(object parameter)
        {
            if (SelectedBackups != null)
            {
                Parallel.ForEach(SelectedBackups, backup =>
                {
                    Thread backupThread = new Thread(() =>
                    {
                        try
                        {
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();

                            backup.ExecuteCopy();
                            backup.Executed = true;
                            stopwatch.Stop();
                            string stateName = stopwatch.IsRunning ? "In Progress" : (stopwatch.ElapsedMilliseconds > 0 ? "Finished" : "Not Started");

                            BackupLogHandler a = new BackupLogHandler();
                            string sourceFilePath = backup.FileSource;

                            int filesAlreadyCopied = backup.CopiedFiles.Count;

                            DirectoryInfo dirInfo = new DirectoryInfo(backup.FileSource);
                            long size = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);

                            long remainingSize = size - backup.CopiedFiles.Sum(file => new FileInfo(file).Length);
                            int totalFilesToCopy = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Count();
                            int remainingFiles = Math.Max(totalFilesToCopy - filesAlreadyCopied, 0);

                            var FileTransferTime = stopwatch.Elapsed.ToString();
                            CreateLog(backup.FileName, backup.FileSource, backup.FileTarget, size, FileTransferTime, "Execute", OutputFormat);
                            StateForBackup(backup.FileName, backup.FileSource, backup.FileTarget, size, filesAlreadyCopied, remainingSize, remainingFiles, stateName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error executing backup '{backup.FileName}': {ex.Message}");
                        }
                    });

                    backupThread.Start();
                });

                MessageBox.Show("Selected backups execution successful.");
                LoadBackups();
            }
            else
            {
                MessageBox.Show("Please select a backup to execute.");
            }
        }

        private void PauseBackup(object parameter)
        {
            if (SelectedBackups != null)
            {
                foreach (var backup in SelectedBackups)
                {
                    backup.IsPaused = true; 
                }
                MessageBox.Show("Selected backups paused.");
            }
        }

        private void ContinueBackup(object parameter)
        {
            if (SelectedBackups != null)
            {
                foreach (var backup in SelectedBackups)
                {
                    backup.IsPaused = false; 
                }
                MessageBox.Show("Selected backups resumed.");
            }
        }

        private void StopBackup(object parameter)
        {
            if (SelectedBackups != null)
            {
                // Arrêtez les sauvegardes en cours et réinitialisez les statistiques
                foreach (var backup in SelectedBackups)
                {
                    backup.Stop();
                }
                MessageBox.Show("Selected backups stopped.");
            }
        }

        public void StateForBackup(string _name, string _source, string _target, long size, int filesAlreadyCopied, long remainingSize, int remainingFiles, string stateName)
        {
            BackupStateHandler a = new BackupStateHandler();
            string sourceFilePath = _source;
            FileInfo fileInfo = new FileInfo(sourceFilePath);
            string[] files = Directory.GetFiles(sourceFilePath);

            var state = new BackupState
            {
                FileName = _name,
                Timestamp = DateTime.Now,
                StateName = stateName,
                TotalFilesToCopy = files.Length,
                TotalFilesSize = size,
                RemainingFiles = remainingFiles,
                RemainingSize = remainingSize,
                FileSource = _source,
                FileTarget = _target,
            };
            a.UpdateState(state);
        }

        public void CreateLog(string _name, string _source, string _target, long size, string FileTransferTime, string details, string outputFormat)
        {
            BackupLogHandler a = new BackupLogHandler();
            string sourceFilePath = _source;
            FileInfo fileInfo = new(sourceFilePath);

            if (_name != null && _source != null && _target != null && FileTransferTime != null)
            {
                var log = new BackupLog
                {
                    FileName = _name,
                    FileSource = _source,
                    FileTarget = _target,
                    FileSize = size,
                    FileTransferTime = FileTransferTime,
                    FileTime = DateTime.Now,
                };
                a.UpdateLog(log, outputFormat);
            }
            else
            {
                MessageBox.Show("One or more values required for log creation are null.");
            }
        }
    }
}
