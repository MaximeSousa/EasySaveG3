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
            this.DataContext = new VMSettings();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItems = (ComboBoxItem)((ComboBox)sender).SelectedItem;
           
            if (selectedItems.Content.ToString() == "French")
            {
                Setting.TraductorFrench();
            }
            else
            {
                Setting.TraductorEnglish();
            }
        }
    }
}
