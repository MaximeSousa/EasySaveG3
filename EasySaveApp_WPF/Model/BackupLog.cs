using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using EasySaveApp_WPF.ViewModel;

namespace EasySaveApp_WPF.Models
{
    // Model representing a backup log entry
    public class BackupLog
    {
        public string FileName { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public string FileTransferTime { get; set; }
        public DateTime FileTime { get; set; }
        public string Details { get; set; }
        public long EncryptionTime { get; set; }

    }

    // Serializable dictionary for XML serialization
    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
    }

    // Class for handling backup logs
    public class BackupLogHandler
    {
        private VMSettings _vmSettings;

        private SerializableDictionary<string, BackupLog> saveLog;
        public BackupLogHandler(VMSettings vmSettings)
        {
            _vmSettings = vmSettings;
            saveLog = new SerializableDictionary<string, BackupLog>();

            if (File.Exists("Log.json"))
            {
                LoadLogFromJson();
            }
            else if (File.Exists("Log.xml"))
            {
                LoadLogFromXml();
            }
        }


        // Method to update the backup log
        public void UpdateLog(BackupLog log)
        {
            saveLog[log.FileName] = log;
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            if (_vmSettings.OutputFormat == "json")
            {
                SaveLogToJson($"Log_{currentDate}.json");
            }
            else
            {
                SaveLogToXml($"Log_{currentDate}.xml");
            }
        }

        // Method to save the backup log to JSON
        public void SaveLogToJson(string fileName)
        {
            string json = JsonConvert.SerializeObject(saveLog, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        // Method to save the backup log to XML
        public void SaveLogToXml(string fileName)
        {
            var serializer = new XmlSerializer(typeof(SerializableDictionary<string, BackupLog>));
            using (var stream = new StreamWriter(fileName))
            {
                serializer.Serialize(stream, saveLog);
            }
        }

        // Method to load the backup log from JSON
        public void LoadLogFromJson()
        {
            if (File.Exists("Log.json"))
            {
                string json = File.ReadAllText("Log.json");
                saveLog = JsonConvert.DeserializeObject<SerializableDictionary<string, BackupLog>>(json);
            }
        }

        // Method to load the backup log from XML
        public void LoadLogFromXml()
        {
            if (File.Exists("Log.xml"))
            {
                var serializer = new XmlSerializer(typeof(SerializableDictionary<string, BackupLog>));
                using (var stream = new StreamReader("Log.xml"))
                {
                    saveLog = (SerializableDictionary<string, BackupLog>)serializer.Deserialize(stream);
                    Console.WriteLine("Log.xml");
                }
            }
        }
    }
}