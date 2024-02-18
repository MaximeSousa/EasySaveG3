using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EasySaveApp_WPF.Models
{
    public class BackupState
    {
        public string FileName { get; set; }
        public DateTime Timestamp { get; set; }
        public string StateName { get; set; }
        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public int RemainingFiles { get; set; }
        public long RemainingSize { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
    }

    public class BackupStateHandler
    {
        public Dictionary<string, BackupState> saveState;

        public BackupStateHandler()
        {
            if (File.Exists("State.json"))
            {
                LoadStateFromJson();
            }
            else
            {
                saveState = new Dictionary<string, BackupState>();
            }
        }

        public void UpdateState(BackupState state)
        {
            saveState[state.FileName] = state;
            SaveStateToJson();
        }

        public void SaveStateToJson()
        {
            string json = JsonConvert.SerializeObject(saveState, Formatting.Indented);
            File.WriteAllText("State.json", json);
        }

        public void LoadStateFromJson()
        {
            if (File.Exists("State.json"))
            {
                string json = File.ReadAllText("state.json");
                saveState = JsonConvert.DeserializeObject<Dictionary<string, BackupState>>(json);
            }
        }
    }
}