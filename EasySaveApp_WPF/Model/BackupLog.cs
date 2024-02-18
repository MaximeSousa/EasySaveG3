using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EasySaveApp_WPF.Models
{
    public class BackupLog
    {
        public string FileName { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public string FileTransferTime { get; set; }
        public DateTime FileTime { get; set; }
    }

    public class BackupLogHandler
    {
        private Dictionary<string, BackupLog> saveLog;

        public BackupLogHandler()
        {
            if (File.Exists("Log.json"))
            {
                LoadLogFromJson();
            }
            else
            {
                saveLog = new Dictionary<string, BackupLog>();
            }
        }

        public void UpdateLog(BackupLog Log)
        {
            saveLog[Log.FileName] = Log;
            SaveLogToJson();
        }

        public void SaveLogToJson()
        {
            string json = JsonConvert.SerializeObject(saveLog, Formatting.Indented);
            File.WriteAllText("Log.json", json);
        }

        public void LoadLogFromJson()
        {
            if (File.Exists("Log.json"))
            {
                string json = File.ReadAllText("Log.json");
                saveLog = JsonConvert.DeserializeObject<Dictionary<string, BackupLog>>(json);
            }
        }
    }
}