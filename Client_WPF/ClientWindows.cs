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

        // Constructor to initialize the dispatcher
        public ClientWindows(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        // Method to start the client
        public void Client()
        {
            Socket socket = Connect();
            Task.Run(async () => await Listen(socket));
        }

        // Method to establish a connection to the server
        private static Socket Connect()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
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

        // Method to listen for messages from the server
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

        // Method to asynchronously receive data from the server
        private async Task<int> ReceiveAsync(Socket socket, byte[] buffer)
        {
            return await Task<int>.Factory.FromAsync(
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, null, socket),
                socket.EndReceive);
        }

        // Method to handle received backup information
        private void OnBackupInfoReceived(string backupName, string BackupBar)
        {
            _dispatcher.Invoke(() =>
            {
                MessageReceived?.Invoke(this, backupName + ":" + BackupBar);

            });
        }
    }
}
