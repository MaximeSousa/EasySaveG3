using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;


namespace Client_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientWindows _client;

        public MainWindow()
        {
            InitializeComponent();
            _client = new ClientWindows(Dispatcher);
            _client.MessageReceived += OnMessageReceived;
            _client.Client();
        }

        private void OnMessageReceived(object sender, string message)
        {
            // Mise à jour de l'interface utilisateur sur le thread de l'interface utilisateur
            Dispatcher.Invoke(() =>
            {
                // Diviser le message en nom de sauvegarde et progrès
                string[] parts = message.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out int progress))
                {
                    // Mettre à jour la barre de progression
                    UpdateProgressBar(parts[0], progress);
                }
                else
                {
                   
                }
            });
        }

        private void UpdateProgressBar(string backupName, int progress)
        {
            if (backupName == "Backup1")
            {
                ProgressBar1.Value = progress;
            }
            else if (backupName == "Backup2")
            {
                ProgressBar2.Value = progress;
            }
        }

       
    }
}
