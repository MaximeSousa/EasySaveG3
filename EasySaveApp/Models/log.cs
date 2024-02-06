using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace EasySaveApp.Models
{
    public class Log
    {
        public string FileName { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public string FileSize { get; set; }
        public string FileTransferTime { get; set; }
        public string FileTime { get; set; }

    }

    public class LogExecute
    {
        public static void ExcutLog()
        {
            var log = new Log
            {
                FileName = "FileName",
                FileSource = "FileSource",
                FileTarget = "FileTarget",
                FileSize = "FileSize",
                FileTransferTime = "FileTransferTime",
                FileTime = "FileTime",
            };
            string fileName = "Log.json";
            string jsonString = JsonSerializer.Serialize(log);
            File.WriteAllText(fileName, jsonString);

            Console.WriteLine(jsonString);
        }
    }
}
