using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasySaveApp.Models;
using System.Diagnostics;

namespace EasySaveApp.ViewsModel
{
    class ViewModel
    {
        public string OutputFormat { get; set; } = "json";
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

            Console.WriteLine("La sauvegarde a été effectuée avec succès !");

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
            CreateLog(_name, _source, _target, size, FileTransferTime, "create", OutputFormat);
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
                RemainingFiles= remainingFiles,
                RemainingSize = remainingSize,
                FileSource = _source,
                FileTarget = _target,
            };
            a.UpdateState(state);
        }

        public void CreateLog(string _name, string _source, string _target, long size, string FileTransferTime, string details, string outputFormat)
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
                Details = details
            };
            a.UpdateLog(log, outputFormat);
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
        public void ExeBackupJob(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.Clear();
                    Console.WriteLine("No backup numbers provided.");
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
                    Console.Clear();
                    Console.WriteLine("No valid backup numbers provided.");
                    return;
                }

                foreach (int backupNumber in backupNumbers)
                {
                    if (backupNumber >= 1 && backupNumber <= BackupFile.backups.Count && BackupFile.backups[backupNumber - 1] != null)
                    {
                        BackupFile backup = BackupFile.backups[backupNumber - 1];
                        backup.ExecuteCopy();
                        Console.Clear();
                        Console.WriteLine($"Backup {backupNumber} executed successfully.");
                        CreateLog(backup.FileName, backup.FileSource, backup.FileTarget, backup.FileSize, backup.FileTransferTime, "execute", OutputFormat);
                    }
                    else
                    {
                        Console.WriteLine($"Backup {backupNumber} does not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
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

        public void ChangeBackup()
        {
            Console.WriteLine("Enter the Name of the backup that you want to modify: ");
            string nameBackupChange = Console.ReadLine();
            var backup = BackupFile.backups.Skip(1).FirstOrDefault(b => b.FileName.Equals(nameBackupChange, StringComparison.OrdinalIgnoreCase));

            if (backup != null)
            {
                Console.WriteLine("Enter the new name for the backup: ");
                string newName = Console.ReadLine();
                string newSource = GetBackupSource();
                string newTarget = GetBackupTarget();
                BackupType newType = GetBackupType();

                string OldBackupDirectory = Path.Combine(backup.FileTarget, backup.FileName);
                string NewBackupDirectory = Path.Combine(backup.FileTarget, newName);

                if (!newName.Equals(backup.FileName, StringComparison.OrdinalIgnoreCase))
                {

                    if (Directory.Exists(OldBackupDirectory))
                    {
                        MoveDirectory(OldBackupDirectory, NewBackupDirectory);
                        OldBackupDirectory = NewBackupDirectory;
                    }
                }

                if (!newSource.Equals(backup.FileSource, StringComparison.OrdinalIgnoreCase))
                {

                    if (Directory.Exists(OldBackupDirectory))
                    {
                        Directory.Delete(OldBackupDirectory, true);
                    }
                   
                    Directory.CreateDirectory(NewBackupDirectory);

                    backup.FileSource = newSource;
                    backup.ExecuteCopy();
                }

                if (!newTarget.Equals(backup.FileTarget, StringComparison.OrdinalIgnoreCase))
                {
                    string newBackupDir = Path.Combine(newTarget, backup.FileName);

                    if (Directory.Exists(OldBackupDirectory))
                    {
                        MoveDirectory(OldBackupDirectory, newBackupDir);
                    }

                    backup.FileTarget = newTarget;
                }


                backup.FileName = newName;
                backup.Type = newType;

                BackupFile.SaveBackupsToFile();

                Console.WriteLine($"Backup '{nameBackupChange}' updated successfully.");
                CreateLog(newName, newSource, newTarget, backup.FileSize, backup.FileTransferTime, "change", OutputFormat);
            }
            else
            {
                Console.WriteLine($"Backup '{nameBackupChange}' does not exist.");
            }
        }

        private void MoveDirectory(string OldDirectory, string NewDirectory)
        {
            if (Directory.Exists(OldDirectory))
            {
                Directory.Move(OldDirectory,NewDirectory);
            }
        }

        public void DeleteBackup()
        {
            Console.WriteLine("Enter the Name of the backup that you want to delete: ");
            string nameBackupDelete = Console.ReadLine();
            var backup = BackupFile.backups.Skip(1).FirstOrDefault(b => b.FileName.Equals(nameBackupDelete, StringComparison.OrdinalIgnoreCase));

            
            if (backup != null)
            {
                string BackupFileDirectory = Path.Combine(backup.FileTarget, backup.FileName);

                if (Directory.Exists(BackupFileDirectory))
                {
                    Directory.Delete(BackupFileDirectory, true);
                }
                BackupFile.backups.Remove(backup);
                BackupFile.SaveBackupsToFile();
                Console.Clear();
                Console.WriteLine($"Backup '{nameBackupDelete}' deleted successfully.");
                CreateLog(backup.FileName, backup.FileSource, backup.FileTarget, backup.FileSize, backup.FileTransferTime, "delete", OutputFormat);
            }
            else
            {
                Console.WriteLine($"Backup '{nameBackupDelete}' does not exist.");
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
