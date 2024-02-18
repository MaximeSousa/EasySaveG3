using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EasySaveApp_WPF.Models;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace EasySaveApp_WPF.ViewModel
{
    public class VMCreateBackup : VMBaseViewModel
    {
        public ICommand CreateBackupCommand { get; }

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
        private readonly string[] AllowedExtensions = { ".txt", ".pdf" };

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

                BackupFile newBackup = BackupFile.CreateBackup(BackupName, Source, Destination, type);

                newBackup.Executed = false;
                if (!Backups.Contains(newBackup))
                {
                    Backups.Add(newBackup);
                    BackupHandler.BackupHandlerInstance.SaveBackupsToJson();
                }
                // Encrypter les fichiers si c'est une sauvegarde complète
                if (IsFullBackup || IsDifferentialBackup)
                {
                    EncryptFiles(Source, Destination);
                }
                BackupName = "";
                Source = "";
                Destination = "";
                IsFullBackup = false;
                IsDifferentialBackup = false;

                MessageBox.Show("Backup created successfully and crypted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create backup: {ex.Message}");
            }
        }
        private void EncryptFiles(string Source, string Destination)
        {
            string[] files = Directory.GetFiles(Source);

            // Parallélisation du chiffrement des fichiers
            Parallel.ForEach(files, file =>
            {
                EncryptFile(file, Destination);
            });
        }
        private void EncryptFile(string filePath, string Destination)
        {
            string fileName = Path.GetFileName(filePath);
            string extension = Path.GetExtension(fileName);

            // Vérifier si l'extension du fichier est autorisée à être chiffrée
            if (AllowedExtensions.Contains(extension.ToLower()))
            {
                string encryptedFilePath = Path.Combine(Destination, fileName + ".encrypted");

                // Clé de chiffrement
                byte[] key = Encoding.UTF8.GetBytes("cledechiffrement");

                using (FileStream sourceStream = File.OpenRead(filePath))
                using (FileStream targetStream = File.Create(encryptedFilePath))
                {
                    int bytesRead;
                    byte[] buffer = new byte[1024];

                    while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // Chiffrement XOR
                        for (int i = 0; i < bytesRead; i++)
                        {
                            buffer[i] = (byte)(buffer[i] ^ key[i % key.Length]);
                        }

                        targetStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

    }
}
