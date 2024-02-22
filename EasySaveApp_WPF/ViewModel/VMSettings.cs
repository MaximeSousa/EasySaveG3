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
        private ResourceManager _resourceManager;
        private CultureInfo _currentCulture;

        private static readonly string METIER_SOFTWARE = "CalculatorApp"; // Logiciel métier

        public ICommand ChangeLanguageToFrenchCommand { get; }
        public ICommand ChangeLanguageToEnglishCommand { get; }
        public ICommand BrowseMetierSoftwareCommand { get; } // Ajout de la commande de navigation
        public ICommand SelectFormat { get; }
        private string _metierSoftwarePath; // Ajout de la propriété de stockage du chemin du logiciel métier
        public string MetierSoftwarePath
        {
            get { return _metierSoftwarePath; }
            set
            {
                _metierSoftwarePath = value;
                OnPropertyChanged(nameof(MetierSoftwarePath));
            }
        }

        public object LocalizedStrings { get; private set; }

        public VMSettings()
        {
            ChangeLanguageToFrenchCommand = new RelayCommand(ChangeLanguageToFrench);
            ChangeLanguageToEnglishCommand = new RelayCommand(ChangeLanguageToEnglish);
            BrowseMetierSoftwareCommand = new RelayCommand(BrowseMetierSoftware); // Initialisation de la commande de navigation
            SelectFormat = new RelayCommand(ConfirmFormat, CanConfirmFormat);

            // Initialisation du gestionnaire de ressources pour les langues
            _resourceManager = new ResourceManager("EasySaveApp_WPF.Resources.Lang.Resources", Assembly.GetExecutingAssembly());
            _currentCulture = CultureInfo.CurrentUICulture;
        }

        private void ChangeLanguageToFrench(object obj)
        {
            ChangeCulture("fr");
        }

        private void ChangeLanguageToEnglish(object obj)
        {
            ChangeCulture("en");
        }

        private void ChangeCulture(string cultureName)
        {
            if (_currentCulture.Name != cultureName)
            {
                _currentCulture = new CultureInfo(cultureName);
                CultureInfo.DefaultThreadCurrentCulture = _currentCulture;
                CultureInfo.DefaultThreadCurrentUICulture = _currentCulture;

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

        private string _outputFormat;
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