using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasySaveApp.Models;
using System.Diagnostics;
using System.Resources;

namespace EasySaveApp.ViewsModel
{
    class ViewModel
    {
        // Variable to store the selected language
        private static string language = "English";
        private static ResourceManager resourceManager;

        public ViewModel()
        {
            InitializeResourceManager();
        }

        private static void InitializeResourceManager()
        {
            string resourceFile = language == "Français" ? "Resource_fr" : "Resource_en";
            resourceManager = new ResourceManager($"EasySaveApp.Resources.{resourceFile}", typeof(ViewModel).Assembly);
        }

        public void CreateExecuteBackup()
        {
            string _name = GetBackupName();
            string _source = GetBackupSource();
            string _target = GetBackupTarget();
            BackupType _type = GetBackupType();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            BackupFile backup = BackupFile.CreateBackup(_name, _source, _target, _type);
            backup.ExecuteCopy();

            stopwatch.Stop();

            Console.WriteLine(resourceManager.GetString("The backup was completed successfully!"));

            string stateName = stopwatch.IsRunning ? "In Progress" : "Finished";


            BackupLogHandler a = new BackupLogHandler();
            string sourceFilePath = Path.Combine(Directory.GetCurrentDirectory());

            int filesAlreadyCopied = backup.CopiedFiles.Count;

            DirectoryInfo dirInfo = new DirectoryInfo(_source);
            long size = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);

            long remainingSize = size - backup.CopiedFiles.Sum(file => new FileInfo(file).Length);
            int totalFilesToCopy = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Count();
            int remainingFiles = Math.Max(totalFilesToCopy - filesAlreadyCopied, 0);

            var FileTransferTime = stopwatch.Elapsed.ToString();
            CreateLog(_name, _source, _target, size, FileTransferTime);
            StateForBackup(_name, _source, _target, size, filesAlreadyCopied, remainingSize, remainingFiles, stateName);
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

        public string GetBackupName()
        {
            Console.WriteLine(resourceManager.GetString("Enter a name for the Backup-15 max"));
            string nameBackup = Console.ReadLine();
            while (nameBackup.Length < 1 || nameBackup.Length > 15)
            {
                Console.Clear();
                Console.WriteLine(resourceManager.GetString("This name is not valid"));
                nameBackup = GetBackupName();
            }
            return nameBackup;
        }

        public void ExeBackupJob(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine(resourceManager.GetString("No backup numbers provided"));
                    return;
                }

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

                backupNumbers = backupNumbers.Distinct().OrderBy(n => n).ToList();

                if (backupNumbers.Count == 0)
                {
                    Console.WriteLine(resourceManager.GetString("No valid backup numbers provided"));
                    return;
                }

                foreach (int backupNumber in backupNumbers)
                {
                    if (backupNumber >= 1 && backupNumber <= BackupFile.backups.Count && BackupFile.backups[backupNumber - 1] != null)
                    {
                        BackupFile backup = BackupFile.backups[backupNumber - 1];
                        backup.ExecuteCopy();
                        Console.WriteLine($"{resourceManager.GetString($"Backup {backupNumber} executed successfully.")}");

                    }
                    else
                    {
                        Console.WriteLine($"{resourceManager.GetString($"Backup {backupNumber} does not exist.")}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{resourceManager.GetString($"An error occurred: {ex.Message}")}");
            }
        }

        public string PathCorrector(string path)
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
            Console.WriteLine(resourceManager.GetString("Enter the Directory source for the backup"));
            string sourceBackup = PathCorrector(Console.ReadLine());

            while (!Directory.Exists(sourceBackup) && sourceBackup != "0")
            {
                Console.Clear();
                Console.WriteLine(resourceManager.GetString("The directory source doesn't exist"));
                sourceBackup = GetBackupSource();

            }
            return sourceBackup;
        }

        public string GetBackupTarget()
        {
            Console.WriteLine(resourceManager.GetString("Enter the Directory target for the backup"));
            string targetBackup = PathCorrector(Console.ReadLine());

            while (!Directory.Exists(targetBackup) && targetBackup != "0")
            {
                Console.Clear();
                Console.WriteLine(resourceManager.GetString("The directory source doesn't exist"));
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
                Console.WriteLine($"{resourceManager.GetString($"Name: {backup.FileName}, Source: {backup.FileSource}, Destination: {backup.FileTarget}, Type: {backup.Type}")}");
            }
        }

        public void ChangeBackup()
        {
            Console.WriteLine(resourceManager.GetString("Enter the Name of the backup that you want to modify:"));
            string nameBackupChange = Console.ReadLine();
            var backup = BackupFile.backups.Skip(1).FirstOrDefault(b => b.FileName.Equals(nameBackupChange, StringComparison.OrdinalIgnoreCase));

            if (backup != null)
            {
                Console.WriteLine(resourceManager.GetString("Enter the new name for the backup:"));
                string newName = Console.ReadLine();
                string newSource = GetBackupSource();
                string newTarget = GetBackupTarget();
                BackupType newType = GetBackupType();

                backup.FileName = newName;
                backup.FileSource = newSource;
                backup.FileTarget = newTarget;
                backup.Type = newType;

                BackupFile.SaveBackupsToFile();

                Console.WriteLine($"{resourceManager.GetString($"Backup '{nameBackupChange}' updated successfully.")}");
                CreateLog(newName, newSource, newTarget, backup.FileSize, backup.FileTransferTime);
            }
            else
            {
                Console.WriteLine($"{resourceManager.GetString($"Backup '{nameBackupChange}' does not exist.")}");
            }
        }

        public void DeleteBackup()
        {
            Console.WriteLine(resourceManager.GetString("Enter the Name of the backup that you want to delete:"));
            string nameBackupDelete = Console.ReadLine();
            var backup = BackupFile.backups.Skip(1).FirstOrDefault(b => b.FileName.Equals(nameBackupDelete, StringComparison.OrdinalIgnoreCase));

            if (backup != null)
            {
                Directory.Delete(Path.Combine(backup.FileTarget, backup.FileName), true);
                BackupFile.backups.Remove(backup);
                BackupFile.SaveBackupsToFile();
                Console.WriteLine($"{resourceManager.GetString($"Backup '{nameBackupDelete}' deleted successfully.")}");
                CreateLog(backup.FileName, backup.FileSource, backup.FileTarget, backup.FileSize, backup.FileTransferTime);
            }
            else
            {
                Console.WriteLine($"{resourceManager.GetString($"Backup '{nameBackupDelete}' does not exist.")}");
            }
        }

        public BackupType GetBackupType()
        {
            Console.WriteLine(resourceManager.GetString("Choose the type of backup:"));
            Console.WriteLine($"1. {resourceManager.GetString("Full")}");
            Console.WriteLine($"2. {resourceManager.GetString("Differential")}");
            string choice = Console.ReadLine();
            Console.Clear();
            return choice switch
            {
                "1" => BackupType.Full,
                "2" => BackupType.Differential,
                _ => throw new Exception(resourceManager.GetString("Invalid choice"))
            };
        }

        private static void ChangeLanguage()
        {
            // If the current language is French, change it to English and vice versa
            if (language == "Français")
            {
                language = "English";
            }
            else
            {
                language = "Français";
            }

            // Update the language variable
            Console.Clear();
            Console.WriteLine($"{resourceManager.GetString($"Language changed to {language}.")}");

            // Load resources in the new language
            InitializeResourceManager();
        }
    }
}
