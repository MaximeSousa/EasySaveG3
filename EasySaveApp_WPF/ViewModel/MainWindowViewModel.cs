using System.Windows.Input;
using System.Windows.Controls;
using EasySaveApp_WPF.View;

namespace EasySaveApp_WPF.ViewModel
{
    public class MainWindowViewModel : VMBaseViewModel
    {
        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public ICommand NavigateToCreateBackupCommand { get; }
        public ICommand NavigateToExecuteBackupCommand { get; }
        public ICommand NavigateToDeleteBackupCommand { get; }
        public ICommand NavigateToHomeBackupCommand { get; }

        public MainWindowViewModel()
        {

            NavigateToCreateBackupCommand = new RelayCommand(NavigateToCreateBackup);
            NavigateToExecuteBackupCommand = new RelayCommand(NavigateToExecuteBackup);
            NavigateToHomeBackupCommand = new RelayCommand(NavigateToHomeBackup);

            // Initially the current page
            CurrentPage = new HomePage();
        }

        private void NavigateToCreateBackup(object obj)
        {
            CurrentPage = new CreatePage();
        }

        private void NavigateToHomeBackup(object obj)
        {
            CurrentPage = new HomePage();
        }

        private void NavigateToExecuteBackup(object obj)
        {
            CurrentPage = new ExecutePage();
        }

    }
}
