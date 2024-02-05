using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

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
<<<<<<< HEAD
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
=======
        public static void MainLog()
        {
            var log = new Log
            {
                FileName = 123,
                FileSource = 123,
                FileTarget = 123,
                FileSize = 123,
                FileTransferTime = 123,
                FileTime = 123,
>>>>>>> c92aa00c5af6aee41994402d870748867685cb3a
            };

            string jsonString = JsonSerializer.Serialize(log);

            Console.WriteLine(jsonString);
        }
    }
}
