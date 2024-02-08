using System;
using System.IO;
using EasySaveApp.Models;

namespace EasySaveApp.ViewsModel
{
    class ViewModel
    {
        public void CreateExecuteBackup()
        {
            string _name = GetBackupName();
            string _source = GetBackupSource();
            string _target = GetBackupTarget();
            BackupType _type = GetBackupType();

            BackupFile backup = BackupFile.CreateBackup(_name, _source, _target, _type);
            backup.ExecuteCopy();
            Console.WriteLine("La sauvegarde a été effectuée avec succès !");
        }
        public string GetBackupName()
        {
            Console.WriteLine("Enter a name for the Backup (15 max)");
            string nameBackup = Console.ReadLine();
            while (nameBackup.Length < 1 || nameBackup.Length > 15)
            {
                Console.Clear();
                Console.WriteLine("This name is not valid");
                nameBackup = GetBackupName();
            }
            return nameBackup;
        }

        private string PathCorrector(string path)
        {
            if (string.IsNullOrEmpty(path) && path != "0")
            {
                path = path.Replace("/", "\\").Trim();

            }
            try
            {
                return Path.IsPathRooted(path) && Directory.Exists(Path.GetPathRoot(path)) ? path : "";
            }
            catch
            {
                return "";
            }
        }

        public string GetBackupSource()
        {
            Console.WriteLine("Enter the Directory source for the backup");
            string sourceBackup = PathCorrector(Console.ReadLine());

            while (!Directory.Exists(sourceBackup) && sourceBackup != "0")
            {
                Console.Clear();
                Console.WriteLine("The directory source doesn't exist");
                sourceBackup = GetBackupSource();

            }
            return sourceBackup;
        }

        public string GetBackupTarget()
        {
            Console.WriteLine("Enter the Directory target for the backup");
            string targetBackup = PathCorrector(Console.ReadLine());

            while (!Directory.Exists(targetBackup) && targetBackup != "0")
            {
                Console.Clear();
                Console.WriteLine("The directory source doesn't exist");
                targetBackup = GetBackupTarget();
            }
            return targetBackup;
        }
        public void DisplayBackups()
        {
            BackupFile.LoadBackupsFromFile();
            for (int i = 1; i < BackupFile.backups.Count; i++)
            {
                var backup = BackupFile.backups[i];
                Console.WriteLine($"Nom: {backup.FileName}, Source: {backup.FileSource}, Destination: {backup.FileTarget}, Type: {backup.Type}");
            }
        }

        public BackupType GetBackupType()
        {
            Console.WriteLine("Choose the type of backup:");
            Console.WriteLine("1. Full");
            Console.WriteLine("2. Differential");
            string choice = Console.ReadLine();
            Console.Clear();
            return choice switch
            {
                "1" => BackupType.Full,
                "2" => BackupType.Differential,
                _ => throw new Exception("Invalid choice")
            };
        }

    }
}
