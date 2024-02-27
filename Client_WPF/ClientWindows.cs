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
            //DialoguerRezo(socket);
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

        //private static void DialoguerRezo(Socket serveur)
        //{
        //    byte[] data = new byte[1024];
        //    int recv = serveur.Receive(data);
        //    string stringData;
        //    stringData = Encoding.UTF8.GetString(data, 0, recv);
        //    Console.WriteLine(stringData);

        //    while (true)
        //    {
        //        var TimeClient = new Stopwatch();
        //        TimeClient.Start();
        //        for (int k = 0; k < 100; k++)
        //        {

        //            data = new byte[1024];
        //            recv = serveur.Receive(data);
        //            stringData = Encoding.UTF8.GetString(data, 0, recv);
        //            string progress = stringData;
        //            Console.WriteLine($"-Serveur: [{progress}] {k}% - {TimeClient.Elapsed:mm\\:ss}");
        //            System.Threading.Thread.Sleep(100); // Simule un traitement
        //            Console.SetCursorPosition(0, Console.CursorTop - 1); // Retour en arrière pour effacer la ligne précédente
        //        }
        //        TimeClient.Stop();
        //        Deco(serveur);
        //        //break;
        //    }
        //}



        private async Task DialoguerRezo(Socket server)
        {
            byte[] data = new byte[1024];
            while (true)
            {
                int recv = await ReceiveAsync(server, data);
                string stringData = Encoding.UTF8.GetString(data, 0, recv);
                OnMessageReceived(stringData);
                await Task.Delay(100);
            }
        }

        private async Task<int> ReceiveAsync(Socket socket, byte[] buffer)
        {
            return await Task<int>.Factory.FromAsync(
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, null, socket),
                socket.EndReceive);
        }

        //private void OnMessageReceived(string message)
        //{
        //    MessageReceived?.Invoke(this, message);
        //}

        private void OnMessageReceived(string message)
        {
            _dispatcher.Invoke(() =>
            {
                MessageReceived?.Invoke(this, message);
            });
        }

        //private static void Deco(Socket socket)
        //{
        //    socket.Close();//  socket.Shutdown(SocketShutdown.Both);
        //}
    }
}
