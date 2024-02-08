using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Console.Clear();
            Console.WriteLine("Enter a name for the Backup (15 max)");
            string nameBackup = Console.ReadLine();
            while (nameBackup.Length < 1 || nameBackup.Length > 15)
            {
                Console.WriteLine("This name is not valid");
                GetBackupName();
            }
            return nameBackup;
        }

        public void ExeBacjupJob(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No backup numbers provided.");
                return;
            }

            List<int> backupNumbers = ParseBackupNumbers(args);

            if (backupNumbers.Count == 0)
            {
                Console.WriteLine("No valid backup numbers provided.");
                return;
            }

            foreach (int backupNumber in backupNumbers)
            {
                if (backupNumber >= 1 && backupNumber <= BackupFile.backups.Count)
                {
                    BackupFile backup = BackupFile.backups[backupNumber - 1];
                    backup.ExecuteCopy();
                    Console.WriteLine($"Backup {backupNumber} executed successfully.");
                }
                else
                {
                    Console.WriteLine($"Backup {backupNumber} does not exist.");
                }
            }
        }

        private List<int> ParseBackupNumbers(string[] args)
        {
            List<int> backupNumbers = new List<int>();

            foreach (string arg in args)
            {
                if (arg.Contains('-'))
                {
                    string[] range = arg.Split('-');
                    if (range.Length == 2 && int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                    {
                        backupNumbers.AddRange(Enumerable.Range(start, end - start + 1));
                    }
                }
                else if (arg.Contains(';'))
                {
                    string[] numbers = arg.Split(';');
                    foreach (string number in numbers)
                    {
                        if (int.TryParse(number, out int num))
                        {
                            backupNumbers.Add(num);
                        }
                    }
                }
                else
                {
                    if (int.TryParse(arg, out int num))
                    {
                        backupNumbers.Add(num);
                    }
                }
            }

            return backupNumbers.Distinct().OrderBy(n => n).ToList();
      }
    

    public string PathCorrector(string path)
        {
            if (string.IsNullOrEmpty(path) && path != "0")
            {
                path = path.Replace("/", "\\");
                path.Trim();
            }
            return path;
        }

        public string GetBackupSource()
        {
            Console.WriteLine("Enter the Directory source for the backup");
            string sourceBackup = PathCorrector(Console.ReadLine());

            while (!Directory.Exists(sourceBackup) && sourceBackup != "0")
            {
                Console.Clear();
                Console.WriteLine("The directory source doesn't exist");
                GetBackupSource();

            }
            return sourceBackup;
        }

        public string GetBackupTarget()
        {
            Console.WriteLine("Enter the Directory target fir the backup");
            string targetBackup = PathCorrector(Console.ReadLine());

            while (!Directory.Exists(targetBackup) && targetBackup != "0")
            {
                Console.WriteLine("The directory source doesn't exist");
                GetBackupTarget();
            }
            return targetBackup;
        }
        public void DisplayBackups()
        {
            BackupFile.LoadBackupsFromFile();
            foreach (var backup in BackupFile.backups)
            {
                Console.WriteLine($"Nom: {backup.FileName}, Source: {backup.FileSource}, Destination: {backup.FileTarget}, Type: {backup.Type}");
            }
        }
        public BackupType GetBackupType()
        {
            Console.WriteLine("Choose the type of backup:");
            Console.WriteLine("1. Full");
            Console.WriteLine("2. Differential");
            string choice = Console.ReadLine();

            return choice switch
            {
                "1" => BackupType.Full,
                "2" => BackupType.Differential,
                _ => throw new Exception("Invalid choice")
            };
        }
    }
}
