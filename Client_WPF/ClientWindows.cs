using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Client_WPF
{
    internal class ClientWindows
    {
        public event EventHandler<string> MessageReceived;
        private readonly Dispatcher _dispatcher;

        public ClientWindows(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Client()
        {
            Socket socket = Seconnecter();
            Task.Run(async () => await DialoguerRezo(socket));
        }

        private static Socket Seconnecter()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050); //ip serveur
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);
            }
            catch (SocketException soEx)
            {
                Console.Error.WriteLine("Impossible de se connecter au serveur!");
                Console.Error.WriteLine(soEx.ToString());
            }
            return server;
        }

        //private async Task DialoguerRezo(Socket server)
        //{
        //    while (true)
        //    {
        //        byte[] data = new byte[1024];
        //        int recv = await ReceiveAsync(server, data);
        //        string stringData = Encoding.UTF8.GetString(data, 0, recv);
        //        //string[] parts = stringData.Split(':');
        //        //if (parts.Length == 4)
        //        //{
        //        //    string backupName = parts[0];
        //        //    string BackupState = parts[1];
        //        //    double percentage = double.Parse(parts[2]);
        //        //    double progress = double.Parse(parts[3]);

        //        //    OnBackupInfoReceived(backupName, progress, BackupState, percentage);
        //        //}
        //        string[] backupMessages = stringData.Split(new string[] { "Backup_" }, StringSplitOptions.RemoveEmptyEntries);
        //        foreach (string backupMessage in backupMessages)
        //        {
        //            string[] parts = backupMessage.Split(':');
        //            if (parts.Length == 4)
        //            {
        //                string backupName = "Backup_" + parts[0];
        //                string backupState = parts[1];
        //                double percentage = double.Parse(parts[2]);
        //                double progress = double.Parse(parts[3]);

        //                OnBackupInfoReceived(backupName, progress, backupState, percentage);
        //            }
        //            else
        //            {
        //                // Le message n'est pas dans le format attendu
        //            }
        //        }
        //        await Task.Delay(100);
        //    }
        //}

        private async Task DialoguerRezo(Socket server)
        {
            byte[] data = new byte[1024];
            while (true)
            {
                int recv = await ReceiveAsync(server, data);
                string stringData = Encoding.UTF8.GetString(data, 0, recv);
                string[] backupMessages = stringData.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string backupMessage in backupMessages)
                {
                    string[] parts = backupMessage.Split(':');
                    if (parts.Length == 2)
                    {
                        string backupName = parts[0];
                        string backupState = parts[1];

                        OnBackupInfoReceived(backupName, backupState);
                    }
                    else
                    {
                        // Le message n'est pas dans le format attendu
                    }
                }
                await Task.Delay(100);
            }
        }

        private async Task<int> ReceiveAsync(Socket socket, byte[] buffer)
        {
            return await Task<int>.Factory.FromAsync(
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, null, socket),
                socket.EndReceive);
        }

        private void OnBackupInfoReceived(string backupName, string BackupState)
        {
            _dispatcher.Invoke(() =>
            {
                MessageReceived?.Invoke(this, backupName + ":" + BackupState);

            });
        }
    }
}
