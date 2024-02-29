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
            Socket socket = Connect();
            Task.Run(async () => await Listen(socket));
        }

        private static Socket Connect()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050); //ip serveur
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);
            }
            catch (SocketException soEx)
            {
                MessageBox.Show("Unable to connect to server!");
                MessageBox.Show(soEx.ToString());
            }
            return server;
        }

        private async Task Listen(Socket server)
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
                        string backupBar = parts[1];
                        OnBackupInfoReceived(backupName, backupBar);
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Format ");
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

        private void OnBackupInfoReceived(string backupName, string BackupBar)
        {
            _dispatcher.Invoke(() =>
            {
                MessageReceived?.Invoke(this, backupName + ":" + BackupBar);

            });
        }
    }
}
