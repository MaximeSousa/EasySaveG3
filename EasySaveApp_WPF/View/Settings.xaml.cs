using System.Windows;
using System.Windows.Controls;
using EasySaveApp_WPF.ViewModel;


namespace EasySaveApp_WPF.View
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>

    public partial class Settings : Page
    {
        VMSettings Setting = new VMSettings();

        public Settings()
        {
            InitializeComponent();
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            Setting.TraductorFrench();
        }

        private void ComboBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {
            Setting.TraductorEnglish();
        }
    }
}
