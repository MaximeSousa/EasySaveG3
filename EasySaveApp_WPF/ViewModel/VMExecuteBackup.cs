using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
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
        public bool isChange
        {
            get { return _isChange; }
            set
            {
                _isChange = value;
                OnPropertyChanged(nameof(isChange));
            }
        }

        public ICommand ChangeBackupCommand { get; private set; }
        public ICommand DeleteBackupCommand { get; private set; }
        public ICommand ExecuteBackupCommand { get; private set; }
        public ICommand VisibleBackupCommand { get; private set; }

        public string BackupName { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public BackupType Type { get; set; }

        public VMExecuteBackup()
        {
            LoadBackups();
            SelectedBackups = new ObservableCollection<BackupFile>();
            ChangeBackupCommand = new RelayCommand(ChangeBackup);
            DeleteBackupCommand = new RelayCommand(DeleteBackup);
            ExecuteBackupCommand = new RelayCommand(ExecuteBackup);
            VisibleBackupCommand = new RelayCommand(Visible);
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
            isChange = true;
        }

        private void ChangeBackup(object parameter)
        {
            if (SelectedBackups != null && SelectedBackups.Count == 1)
            {
                var backup = SelectedBackups[0];

                string oldFileName = backup.FileName;
                string oldFileSource = backup.FileSource;
                string oldFileTarget = Path.Combine(backup.FileTarget,oldFileName);

                try
                {
                    if (!string.IsNullOrEmpty(BackupName) && BackupName != backup.FileName)
                    {
                        string newFileName = BackupName;
                        string newFolderPath = Path.Combine(backup.FileTarget, newFileName);
                        if (Directory.Exists(oldFileTarget))
                        {
                            Directory.Move(oldFileTarget, newFolderPath);
                            backup.FileName = newFileName;
                            backup.FileTarget = newFolderPath;
                        }
                        else
                        {
                            MessageBox.Show("The backup folder does not exist.");
                            return;
                        }
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

                    if (!backup.Executed)
                    {
                        backup.ExecuteCopy();
                    }
                    BackupHandler.BackupHandlerInstance.UpdateBackup(backup);

                    MessageBox.Show("Backup modification successful.");
                    LoadBackups();
                    isChange = false;
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"I/O error while modifying backup '{oldFileName}': {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Permission denied while modifying backup '{oldFileName}': {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error modifying backup '{oldFileName}': {ex.Message}");
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
                foreach (var backup in SelectedBackups)
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
                        CreateLog(backup.FileName, backup.FileSource, backup.FileTarget, size, FileTransferTime);
                        StateForBackup(backup.FileName, backup.FileSource, backup.FileTarget, size, filesAlreadyCopied, remainingSize, remainingFiles, stateName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error executing backup '{backup.FileName}': {ex.Message}");
                    }
                }

                MessageBox.Show("Selected backups execution successful.");
                LoadBackups();
            }
            else
            {
                MessageBox.Show("Please select a backup to execute.");
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

        public void CreateLog(string _name, string _source, string _target, long size, string FileTransferTime)
        {
            BackupLogHandler a = new BackupLogHandler();
            string sourceFilePath = _source;
            FileInfo fileInfo = new FileInfo(sourceFilePath);

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
                a.UpdateLog(log);
            }
            else
            {
                MessageBox.Show("One or more values required for log creation are null.");
            }
        }
    }
}
