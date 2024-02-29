using System.Windows;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

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

        // Event handler for when a message is received from the client
        private void OnMessageReceived(object sender, string message)
        {
            Dispatcher.Invoke(() =>
            {
                string[] parts = message.Split(':');
                if (parts.Length == 2)
                {
                    string backupName = parts[0];
                    string backupbar = parts[1];
                    var existingBackup = backupModels.FirstOrDefault(b => b.BackupName == backupName);
                    if (existingBackup != null)
                    {
                        existingBackup.BackupBar = backupbar;
                    }
                    else
                    {
                        backupModels.Add(new BackupModel { BackupName = backupName, BackupBar = backupbar });
                    }
                }
                else
                {
                    MessageBox.Show("Incorrect Format ");
                }
            });
        }
    }

    // Model class for backup data
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

        private string _backupBar;
        public string BackupBar
        {
            get { return _backupBar; }
            set
            {
                _backupBar = value;
                OnPropertyChanged(nameof(BackupBar));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
