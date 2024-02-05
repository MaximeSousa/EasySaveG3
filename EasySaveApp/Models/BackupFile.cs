using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EasySaveApp.Models
{
    class BackupFile
    {

        public string BackupName { get; set; }
        public string BackupSource { get; set; }
        public string BackupTarget { get; set; }
        public string BackupType { get; set; }

        public static List<BackupFile> backup = new List<BackupFile>();
        public const int NumberMaxOfSave = 5;

        public BackupFile(string BackupName, string BackupSource, string BackupTarget, string BackupType)
        {
            this.BackupName = BackupName;
            this.BackupSource = BackupSource;
            this.BackupTarget = BackupTarget;
            this.BackupType = BackupType;
        }

        //        public static BackupFile CreateBackup(string BackupName, string BackupSource, string BackupTarget, BackupType BackupType)
        //        {
        //            if (backup.Count > NumberMaxOfSave)
        //                throw new Exception("Maximum number of Backup reached");
        //               
        //  
        //        }
        public void FullBackup()
        {

            foreach (var FilePath in Directory.GetFiles(BackupSource))
            {
                var FileName = Path.GetFileName(FilePath);
                File.Copy(FilePath, Path.Combine(BackupTarget, FileName), true);
            }

            foreach (var directoryPath in Directory.GetDirectories(BackupSource))
            {
                var directoryName = Path.GetFileName(directoryPath);
                var newDirectoryTarget = Path.Combine(BackupTarget, directoryName);
                Directory.CreateDirectory(newDirectoryTarget);

                var subDirectoryBackup = new BackupFile(BackupName, directoryPath, newDirectoryTarget, BackupType);
                subDirectoryBackup.FullBackup();
            }
        }
    }
}
