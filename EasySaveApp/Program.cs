using System;
using EasySaveApp.Models;
using EasySaveApp.Views;

namespace EasySaveApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to EasySave !");
            var backup = new BackupFile("Backup1", @"C:\Users\antoi\Documents\test\1\", @"C:\Users\antoi\Documents\test\2\", "FULL");
            backup.FullBackup();

            BackupLogHandler a = new BackupLogHandler();

            var log = new BackupLog
            {
                FileName = "FileName",
                FileSource = "FileSource",
                FileTarget = "FileTarget",
                FileSize = "FileSize",
                FileTransferTime = "FileTransferTime",
                FileTime = "FileTime",
            };

            a.UpdateState(log); // Update the log state before saving
            a.SaveLogToJson();

            // Use the same instance to load the log
            a.LoadLogFromJson();

            //var view = new View();
            //view.GetBackupName();
            //view.GetBackupSource();
            //view.GetBackupTarget();
            Console.WriteLine("La sauvegarde a été effectuée avec succès !");
        }
    }
}
