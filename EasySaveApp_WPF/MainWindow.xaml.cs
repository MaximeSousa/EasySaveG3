using System.Windows;
using System;
using EasySaveApp_WPF.Models;
using EasySaveApp_WPF.ViewModel;
using System.Windows.Threading;

namespace EasySaveApp_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Server _server;
        private VMExecuteBackup executeModel;
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((sender, e) => BackupFile.MonitorProcess());
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            DataContext = new MainWindowViewModel();
            _server = new Server(executeModel);
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri("Resources/DictionaryEnglish.xaml", UriKind.RelativeOrAbsolute);
        }

    }
}
