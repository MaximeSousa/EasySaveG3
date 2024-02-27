using System;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Windows.Input;
using EasySaveApp_WPF.ViewModel;
using System.Configuration;
using System.Windows;


namespace EasySaveApp_WPF.ViewModel
{
    public class VMSettings : VMBaseViewModel
    {
        public VMSettings()
        {

        }

        public void TraductorEnglish()
        {
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri("/Resources/DictionaryEnglish.xaml", UriKind.RelativeOrAbsolute);
        }
        public void TraductorFrench()
        {
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri("/Resources/DictionaryFrench.xaml", UriKind.RelativeOrAbsolute);
        }

        private bool _isXmlSelected;
        public bool IsXmlSelected
        {
            get { return _isXmlSelected; }
            set
            {
                _isXmlSelected = value;
                OnPropertyChanged("IsXmlSelected");
                if (value) OutputFormat = "xml";
            }
        }

        private bool _isJsonSelected;
        public bool IsJsonSelected
        {
            get { return _isJsonSelected; }
            set
            {
                _isJsonSelected = value;
                OnPropertyChanged("IsJsonSelected");
                if (value) OutputFormat = "json";
            }
        }

        

        public string _outputFormat;
        public string OutputFormat
        {
            get { return _outputFormat; }
            set
            {
                _outputFormat = value;
                OnPropertyChanged("OutputFormat");

            }
        }

        private void ConfirmFormat(object parameter)
        {
            MessageBox.Show($"Output format changed to {OutputFormat}.");
        }

        private bool CanConfirmFormat(object parameter)
        {
            return OutputFormat == "xml" || OutputFormat == "json";
        }

    }
}