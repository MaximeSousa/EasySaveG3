using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EasySaveApp_WPF.Models;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.Linq;

namespace EasySaveApp_WPF.ViewModel
{
    public class VMCreateBackup : VMBaseViewModel
    {
        public ICommand CreateBackupCommand { get; }
        public ICommand BrowseSourceCommand { get; }
        public ICommand BrowseTargetCommand { get; }

        private string _backupName;
        public string BackupName
        {
            get { return _backupName; }
            set
            {
                _backupName = value;
                OnPropertyChanged(nameof(BackupName));
            }
        }

        private string _source;
        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        private string _destination;
        public string Destination
        {
            get { return _destination; }
            set
            {
                _destination = value;
                OnPropertyChanged(nameof(Destination));
            }
        }

        private bool _isFullBackup;
        public bool IsFullBackup
        {
            get { return _isFullBackup; }
            set
            {
                _isFullBackup = value;
                OnPropertyChanged(nameof(IsFullBackup));
            }
        }

        private bool _isDifferentialBackup;
        public bool IsDifferentialBackup
        {
            get { return _isDifferentialBackup; }
            set
            {
                _isDifferentialBackup = value;
                OnPropertyChanged(nameof(IsDifferentialBackup));
            }
        }

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
        public VMCreateBackup()
        {
            LoadBackups();
            CreateBackupCommand = new RelayCommand(CreateBackup);
            BrowseSourceCommand = new RelayCommand(BrowseSource);
            BrowseTargetCommand = new RelayCommand(BrowseTarget);
        }

        private void BrowseSource(object obj)
        {
            OpenFileDialog FolderDialog = new();

            FolderDialog.CheckFileExists = false;
            FolderDialog.FileName = "Source Folder";

            if (FolderDialog.ShowDialog() == true)
            {
                // Récupérer le chemin du dossier sélectionné
                string FolderPath = Path.GetDirectoryName(FolderDialog.FileName);
                Source = FolderPath;
            }
        }

        private void BrowseTarget(object obj)
        {
            OpenFileDialog FolderDialog = new();

            FolderDialog.CheckFileExists = false;
            FolderDialog.FileName = "Target Folder";

            if (FolderDialog.ShowDialog() == true)
            {
                // Récupérer le chemin du dossier sélectionné
                string FolderPath = Path.GetDirectoryName(FolderDialog.FileName);
                Destination = FolderPath;
            }
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
                MessageBox.Show($"Erreur lors du chargement des sauvegardes : {ex.Message}");
            }
        }

        private void CreateBackup(object obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BackupName) || BackupName.Length > 15 || string.IsNullOrWhiteSpace(Source) || string.IsNullOrWhiteSpace(Destination) || Source == Destination)
                {
                    throw new ArgumentException("Backup name cannot be empty or more that 15 characters." + "Source and Target cannot be empty or the same.");
                }

                if (!Directory.Exists(Source))
                {
                    throw new DirectoryNotFoundException("Source path is not valid.");
                }

                if (!Directory.Exists(Destination))
                {
                    throw new DirectoryNotFoundException("Target path is not valid.");
                }

                if (!IsFullBackup && !IsDifferentialBackup)
                {
                    throw new ArgumentException("Please select the backup type (Full or Differential).");
                }

                LoadBackups();
                BackupType type = IsFullBackup ? BackupType.Full : BackupType.Differential;

                BackupFile newBackup = BackupFile.CreateBackup(BackupName, Source, Destination, type, false);

                newBackup.Executed = false;
                if (!Backups.Contains(newBackup))
                {
                    Backups.Add(newBackup);
                    BackupHandler.BackupHandlerInstance.SaveBackupsToJson();
                }

                VMSettings settings = new VMSettings();
                ObservableCollection<ExtensionItem> allowedExtensions = settings.AllowedExtensions;

                string[] allFiles = Directory.GetFiles(Source, "*", SearchOption.AllDirectories);

                // Exécution en parallèle
                Parallel.ForEach(allFiles, filePath =>
                {
                    string fileExtension = Path.GetExtension(filePath);

                    // Vérifier si l'extension du fichier est autorisée
                    if (allowedExtensions.Any(ext => ext.Extension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
                    {
                        // Appeler l'exécutable CryptoSoft pour crypter les fichiers
                        string cryptoSoftPath = @"C:\Users\akiza\source\repos\MaximeSousa\EasySaveG3\CryptoSoft\bin\Debug\net5.0\CryptoSoft.exe";
                        string arguments = $"\"{Source}\" \"{Destination}\" \"(x:W$\"";

                        ProcessStartInfo startInfo = new ProcessStartInfo(cryptoSoftPath, arguments);
                        startInfo.CreateNoWindow = true;
                        startInfo.UseShellExecute = false;

                        using (Process process = Process.Start(startInfo))
                        {
                            process.WaitForExit();
                        }
                    }
                    else
                    {
                        // Afficher un message indiquant que le fichier n'a pas été crypté en raison de son extension non autorisée
                        //MessageBox.Show($"Le fichier '{filePath}' n'a pas été crypté car son extension n'est pas autorisée.");
                    }
                });

                Source = "";
                Destination = "";
                IsFullBackup = false;
                IsDifferentialBackup = false;

                MessageBox.Show("Backup created successfully .");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create backup: {ex.Message}");
            }
        }
        
    }
}