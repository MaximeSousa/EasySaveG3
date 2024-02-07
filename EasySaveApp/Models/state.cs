
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace EasySaveApp.Models
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
    //Gere l'etat des travaux de sauvegarde et chargement vers un fichier json
    public class BackupStateHandler
    {
        private Dictionary<string, BackupState> saveState;
        public BackupStateHandler()
        {
            saveState = new Dictionary<string, BackupState>();
        }
        //met à jour le travail de sauvegarde
        public void UpdateState(BackupState state )
        {
            saveState[state.FileName] = state;
            SaveStateToJson();
        }
        public void SaveStateToJson()
        {
            string json = JsonConvert.SerializeObject(saveState, Formatting.Indented);
            File.WriteAllText("state.json", json);
        }
        public void LoadStateFromJson()
        {
            if (File.Exists("state.json"))
            {
                string json = File.ReadAllText("state.json");
                saveState = JsonConvert.DeserializeObject<Dictionary<string, BackupState>>(json);
            }
        }
    }
}