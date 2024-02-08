using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace EasySaveApp.Models
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
        //private string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log.json");

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
            Console.WriteLine("Log.json");
        }


        public void UpdateLog(BackupLog Log)
        {
            //saveLog = new Dictionary<string, BackupLog>();
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

