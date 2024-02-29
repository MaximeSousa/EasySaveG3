using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasySaveApp_WPF.Models
{
    internal class Server
    {
        public Server()
        {
            _ = Connect(); // Start the server when an instance of Server is created
        }

        // Method to establish a connection and start the server
        public Socket Connect()
        {
            try
            {
                IPEndPoint ipServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050); // IP et port du serveur
                Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                newSocket.Bind(ipServer);
                newSocket.Listen(10);
                // Start accepting connections asynchronously
                Task.Run(() => AcceptConnections(newSocket));
                return newSocket;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du démarrage du serveur : " + ex.Message);
                return null;
            }
        }

        // Method to accept incoming connections
        private void AcceptConnections(Socket newSocket)
        {
            while (true)
            {
                try
                {
                    Socket clientSocket = newSocket.Accept();
                    // Handle the connection in a separate task
                    Task.Run(() => Listen(clientSocket));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de l'acceptation de la connexion client : " + ex.Message);
                }
            }
        }

        // Method to disconnect a client
        public static void Disconnect(Socket socket)
        {
            MessageBox.Show(string.Format("Déconnexion de {0}", ((IPEndPoint)socket.RemoteEndPoint).Address));
            socket.Close();
        }

        // Method to listen to network messages from the client
        static async void Listen(Socket client)
        {
            try
            {
                while (true)
                {
                    ExecuteBackupInfo backupInfo = GetExecuteBackupInfo();
                    await SendRunningBackups(client, backupInfo);
                    await Task.Delay(1000);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la communication avec le client : " + ex.Message);
            }
        }

        // Method to retrieve information about running backups
        static ExecuteBackupInfo GetExecuteBackupInfo()
        {
            List<string> runningBackups = new List<string>();
            List<string> backupStates = new List<string>();

            // Récupérer les noms de fichiers des sauvegardes sauvegardées
            foreach (var backup in BackupHandler.BackupHandlerInstance._saveBackups)
            {
                runningBackups.Add(backup.FileName);
            }

            // Récupérer les états de sauvegarde correspondants
            BackupStateHandler backupStateHandler = new BackupStateHandler();
            foreach (var backup in BackupHandler.BackupHandlerInstance._saveBackups)
            {
                if (backupStateHandler.saveState.ContainsKey(backup.FileName))
                {
                    backupStates.Add(backupStateHandler.saveState[backup.FileName].StateName);
                }
                else
                {
                    backupStates.Add("Not Started");
                }
            }
            return new ExecuteBackupInfo { RunningBackups = runningBackups, BackupStates = backupStates };
        }

        static List<string> previousBackupList = new List<string>();
        static List<string> previousBackupStates = new List<string>();

        // Method to send running backups information to the client
        static async Task SendRunningBackups(Socket client, ExecuteBackupInfo backupInfo)
        {
            // Vérifiez si la liste des sauvegardes à envoyer est identique à la liste précédente
            if (backupInfo.RunningBackups.SequenceEqual(previousBackupList) &&
                backupInfo.BackupStates.SequenceEqual(previousBackupStates))
            {
                // Si les listes sont identiques, ne renvoyez pas les sauvegardes
                return;
            }

            // Mettez à jour les listes précédentes
            previousBackupList = backupInfo.RunningBackups.ToList();
            previousBackupStates = backupInfo.BackupStates.ToList();

            StringBuilder sb = new StringBuilder();
            // Envoyez les sauvegardes mises à jour au client
            for (int i = 0; i < backupInfo.RunningBackups.Count; i++)
            {
                string backupName = backupInfo.RunningBackups[i];
                string backupState = backupInfo.BackupStates[i];
                string message = $"{backupName}:{backupState}";

                sb.Append(message).Append("|");
            }
            SendClient(client, sb.ToString()); // Envoyer le nom de la sauvegarde et son état
            await Task.Delay(100);
        }

        // Method to send data to the clien
        static void SendClient(Socket client, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, SocketFlags.None);
        }
    }

    // Model representing information about running backups
    public class ExecuteBackupInfo
    {
        public List<string> RunningBackups { get; set; }
        public List<string> BackupStates { get; set; }
    }
}
