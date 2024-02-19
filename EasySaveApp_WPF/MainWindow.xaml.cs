using System.Windows;
using System;
using EasySaveApp_WPF.ViewModel;

namespace EasySaveApp_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri("Resources/DictionaryEnglish.xaml", UriKind.RelativeOrAbsolute);
        }

    }
}
