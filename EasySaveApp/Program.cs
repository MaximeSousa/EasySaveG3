using System;
using EasySaveApp.Models;

namespace EasySaveApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to EasySave !");
            var backup = new BackupFile("Backup1", @"", @"", "FULL");
            backup.FullBackup();

            Console.WriteLine("La sauvegarde a été effectuée avec succès !");
        }
    }
}
