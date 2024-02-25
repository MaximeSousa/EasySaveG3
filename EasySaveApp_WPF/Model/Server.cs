using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using EasySaveApp_WPF.Models;

namespace EasySaveApp_WPF.Model
{
    internal class Server
    {
        private Socket _server;

        public Server()
        {
            //_server = Connect();
        }

        public static Socket Connect()
        {
            try
            {
                IPEndPoint ipServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050); // IP et port du serveur
                Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                newSocket.Bind(ipServer);
                newSocket.Listen(10);

                MessageBox.Show("Serveur à l'écoute sur l'adresse 127.0.0.1:9050");

                // Accepter la première connexion entrante
                Socket clientSocket = newSocket.Accept();

                // Gérer la connexion client dans un thread séparé
                Task.Run(() => EcouterReseau(clientSocket));

                return newSocket;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du démarrage du serveur : " + ex.Message);
                return null;
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
                ExecuteBackupInfo backupInfo = GetExecuteBackupInfo();
                await SendRunningBackups(client, backupInfo);
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la communication avec le client : " + ex.Message);
            }
        }

        static ExecuteBackupInfo GetExecuteBackupInfo()
        {
            List<(string FileName, int Progress)> runningBackups = new List<(string, int)>();

            // Ajout de deux sauvegardes fictives avec leur progression
            runningBackups.Add(("Backup1",100));
            runningBackups.Add(("Backup2",80));

            return new ExecuteBackupInfo { RunningBackups = runningBackups };
        }

        //static async Task SendRunningBackups(Socket client, ExecuteBackupInfo backupInfo)
        //{
        //    foreach (var backup in backupInfo.RunningBackups)
        //    {
        //        SendClient(client, backup.FileName, backup.Progress);
        //        await Task.Delay(100);
        //    }
        //}

        //static async Task SendRunningBackups(Socket client, ExecuteBackupInfo backupInfo)
        //{
        //    foreach (var backup in backupInfo.RunningBackups)
        //    {
        //        for (int progress = 0; progress <= backup.Progress; progress++)
        //        {
        //            SendClient(client, backup.FileName, progress);
        //            await Task.Delay(100);
        //        }
        //    }
        //}
        static async Task SendRunningBackups(Socket client, ExecuteBackupInfo backupInfo)
        {
            List<Task> tasks = new List<Task>();

            foreach (var backup in backupInfo.RunningBackups)
            {
                tasks.Add(Task.Run(async () =>
                {
                    for (int progress = 0; progress <= backup.Progress; progress++)
                    {
                        SendClient(client, backup.FileName, progress); // Envoyer la progression de chaque sauvegarde individuellement
                        await Task.Delay(100);
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

        static void SendClient(Socket client, string backupName, int progress)
        {
            string message = $"{backupName}:{progress}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, SocketFlags.None);
        }
    }

    public class ExecuteBackupInfo
    {
        public List<(string FileName, int Progress)> RunningBackups { get; set; }
    }
}
