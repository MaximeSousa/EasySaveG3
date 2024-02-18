using System.Collections.ObjectModel;
using System.Windows.Controls;
using EasySaveApp_WPF.Models;
using EasySaveApp_WPF.ViewModel;

namespace EasySaveApp_WPF.View
{
    /// <summary>
    /// Logique d'interaction pour ExecutePage.xaml
    /// </summary>
    public partial class ExecutePage : Page
    {

        private readonly VMExecuteBackup _viewModel;

        public ExecutePage()
        {
            InitializeComponent();
            _viewModel = new VMExecuteBackup();
            DataContext = _viewModel;
        }

        private void Backup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Backup.SelectedItem != null && Backup.SelectedItem is BackupFile selectedBackup)
            {
                if (_viewModel.SelectedBackups == null)
                {
                    _viewModel.SelectedBackups = new ObservableCollection<BackupFile>();
                }
                else
                {
                    _viewModel.SelectedBackups.Clear();
                }
                foreach (var selectedItem in Backup.SelectedItems)
                {
                    if (selectedItem is BackupFile backupItem)
                    {
                        _viewModel.SelectedBackups.Add(backupItem);
                    }
                }
            }
        }
    }
}
