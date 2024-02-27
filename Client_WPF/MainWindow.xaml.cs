using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace Client_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientWindows _client;
        private ObservableCollection<BackupModel> backupModels = new ObservableCollection<BackupModel>();
        public MainWindow()
        {
            InitializeComponent();
            BackupListView.ItemsSource = backupModels;
            _client = new ClientWindows(Dispatcher);
            _client.MessageReceived += OnMessageReceived;
            _client.Client();
        }

        private void OnMessageReceived(object sender, string message)
        {
            // Mise à jour de l'interface utilisateur sur le thread de l'interface utilisateur
            Dispatcher.Invoke(() =>
            {
                string[] parts = message.Split(':');
                if (parts.Length == 2)
                {
                    string backupName = parts[0];
                    string backupstate = parts[1];

                    var existingBackup = backupModels.FirstOrDefault(b => b.BackupName == backupName);
                    if (existingBackup != null)
                    {
                        existingBackup.BackupState = backupstate;
                    }
                    else
                    {
                        backupModels.Add(new BackupModel { BackupName = backupName, BackupState = backupstate });
                    }
                }
                else
                {
                    // Le message n'est pas dans le format attendu
                }
            });
        }
    }
    public class BackupModel : INotifyPropertyChanged
    {
        private string _backupName;
        public string BackupName
        {
            get { return _backupName; }
            set
            {
                _backupName = value;
                OnPropertyChanged(nameof(BackupName));
            }
        }

        private string _backupState;
        public string BackupState
        {
            get { return _backupState; }
            set
            {
                _backupState = value;
                OnPropertyChanged(nameof(BackupState));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
