using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Windows.Input;

namespace EasySaveApp_WPF.ViewModel
{
    public class VMSettings : VMBaseViewModel
    {
        private ResourceManager _resourceManager;
        private CultureInfo _currentCulture;

        private static readonly string METIER_SOFTWARE = "CalculatorApp"; // Logiciel métier

        public ICommand ChangeLanguageToFrenchCommand { get; }
        public ICommand ChangeLanguageToEnglishCommand { get; }
        public ICommand BrowseMetierSoftwareCommand { get; } // Ajout de la commande de navigation

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

                // Actualisation des ressources dans l'interface utilisateur
                OnPropertyChanged(nameof(LocalizedStrings));
            }
        }

        public string this[string key]
        {
            get
            {
                return _resourceManager.GetString(key, _currentCulture);
            }
        }

        private void BrowseMetierSoftware(object obj)
        {
            // Ouvrir une boîte de dialogue de navigation pour sélectionner le fichier du logiciel métier
            // Par exemple :
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Exécutables (*.exe)|*.exe";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            openFileDialog.Title = "Sélectionner le logiciel métier";

            if (openFileDialog.ShowDialog() == true)
            {
                MetierSoftwarePath = openFileDialog.FileName;
            }
        }


        // Méthode pour vérifier si le logiciel métier est en cours d'exécution
        private bool IsMetierSoftwareRunning()
        {
            Process[] processes = Process.GetProcessesByName(METIER_SOFTWARE);
            return processes.Length > 0;
        }
    }
}
