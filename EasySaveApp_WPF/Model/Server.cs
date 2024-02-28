using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using EasySaveApp_WPF.ViewModel;

namespace EasySaveApp_WPF.Models
{
    internal class Server
    {
        private Socket _server;
        private VMExecuteBackup _executeModel;
        public Server(VMExecuteBackup executeModel)
        {
            _executeModel = executeModel;
            _server = Connect();
        }

        public Socket Connect()
        {
            try
            {
                IPEndPoint ipServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050); // IP et port du serveur
                Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                newSocket.Bind(ipServer);
                newSocket.Listen(10);
                Task.Run(() => AcceptConnections(newSocket));
                return newSocket;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du démarrage du serveur : " + ex.Message);
                return null;
            }
        }

        private void AcceptConnections(Socket newSocket)
        {
            while (true)
            {
                try
                {
                    Socket clientSocket = newSocket.Accept();
                    Task.Run(() => EcouterReseau(clientSocket));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de l'acceptation de la connexion client : " + ex.Message);
                }
            }
        }

        public static void Deconnecter(Socket socket)
        {
            MessageBox.Show(string.Format("Déconnexion de {0}", ((IPEndPoint)socket.RemoteEndPoint).Address));
            socket.Close();
        }

        static async void EcouterReseau(Socket client)
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

        //static ExecuteBackupInfo GetExecuteBackupInfo()
        //{
        //    List<string> runningBackups = new List<string>();
        //    List<string> backupStates = new List<string>();
        //    List<int> progressList = new List<int>();
        //    List<double> percentageList = new List<double>();

        //    // Récupérer les noms de fichiers des sauvegardes sauvegardées
        //    foreach (var backup in BackupHandler.BackupHandlerInstance._saveBackups)
        //    {
        //        runningBackups.Add(backup.FileName);

        //        // Calculer la progression et le pourcentage
        //        long bytesCopied = backup.BytesCopied;
        //        long totalBytes = backup.TotalBytes;
        //        int progress = (int)((bytesCopied * 100) / totalBytes);
        //        double percentage = (bytesCopied * 100.0) / totalBytes;

        //        progressList.Add(progress);
        //        percentageList.Add(percentage);
        //    }

        //    // Récupérer les états de sauvegarde correspondants
        //    BackupStateHandler backupStateHandler = new BackupStateHandler();
        //    foreach (var backup in BackupHandler.BackupHandlerInstance._saveBackups)
        //    {
        //        if (backupStateHandler.saveState.ContainsKey(backup.FileName))
        //        {
        //            backupStates.Add(backupStateHandler.saveState[backup.FileName].StateName);
        //        }
        //        else
        //        {
        //            backupStates.Add("No State");
        //        }
        //    }
        //    return new ExecuteBackupInfo { RunningBackups = runningBackups, BackupStates = backupStates, ProgressList = progressList, PercentageList = percentageList };
        //}

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
                    backupStates.Add("No State");
                }
            }
            return new ExecuteBackupInfo { RunningBackups = runningBackups, BackupStates = backupStates };
        }

        static List<string> previousBackupList = new List<string>();
        static List<string> previousBackupStates = new List<string>();

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

        static void SendClient(Socket client, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, SocketFlags.None);
        }
    }

    public class ExecuteBackupInfo
    {
        public List<string> RunningBackups { get; set; }
        public List<string> BackupStates { get; set; }
    }
}
