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
            Console.Clear();
            Console.WriteLine("Enter a name for the Backup (15 max)");
            string nameBackup = Console.ReadLine();
            while (nameBackup.Length < 1 || nameBackup.Length > 15)
            {
                Console.Clear();
                Console.WriteLine("This name is not valid");
                GetBackupName();
            }
            return nameBackup;
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
            Console.WriteLine(sourceBackup);

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
