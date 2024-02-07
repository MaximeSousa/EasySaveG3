using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json;

namespace EasySaveApp.Models
{   
    public class BackupLog
    {
        public string FileName { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public string FileSize { get; set; }
        public string FileTransferTime { get; set; }
        public string FileTime { get; set; }

    }


    public class BackupLogHandler
    {
        private Dictionary<string, BackupLog> saveLog;
        public BackupLogHandler()
        {
            saveLog = new Dictionary<string, BackupLog>();
        }

        //met à jour le travail de sauvegarde
        public void UpdateState(BackupLog Log)
        {
            saveLog[Log.FileName] = Log;
            SaveLogToJson();
        }
        public void SaveLogToJson()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());

            string currentDirectory = Directory.GetCurrentDirectory();
            string parentDirectory = Directory.GetParent(currentDirectory).FullName;
            string directoryPath = Path.Combine(parentDirectory, "EasySaveApp");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string folderPath = Path.Combine(directoryPath, "Log.json");

            if (!File.Exists(folderPath))
            {
                using (File.Create(folderPath))
                {

                }
            }
            string json = JsonConvert.SerializeObject(saveLog, Formatting.Indented);
            File.WriteAllText(folderPath, json);
        }

        public void LoadLogFromJson()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "EasySaveApp", "Log.json");
            if (File.Exists(folderPath))
            {
                string json = File.ReadAllText(folderPath);
                saveLog = JsonConvert.DeserializeObject<Dictionary<string, BackupLog>>(json);
            }
        }
    }
}


