using System;
using System.IO;

namespace EasySaveApp.Views
{
    class View
    {
    public string GetBackupName()
        {
            Console.Clear();
            Console.WriteLine("Enter a name for the Backup");
            string nameBackup = Console.ReadLine();

            //while (!NameAvailable(nameBackup))
            //{
            //    nameBackup = Console.ReadLine();
            //}
            return nameBackup;
        }

        public string PathCorrector(string path)
        {
           if(path.Length >=1 && path != "0")
            {
                path = path.Replace("/", "\\");
            }
            return path;
        }
    
        public string GetBackupSource()
        {
            Console.WriteLine("Enter the Directory source fir the backup");
            string sourceBackup = PathCorrector(Console.ReadLine());

            while(!Directory.Exists(sourceBackup) && sourceBackup != "0")
            {
                Console.WriteLine("The directory source doesn't exist");
                sourceBackup = PathCorrector(Console.ReadLine());
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
                targetBackup = PathCorrector(Console.ReadLine());
            }
            return targetBackup;
        }
    }
}
