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

        public ICommand ChangeLanguageToFrenchCommand { get; }
        public ICommand ChangeLanguageToEnglishCommand { get; }

        public VMSettings()
        {
            ChangeLanguageToFrenchCommand = new RelayCommand(ChangeLanguageToFrench);
            ChangeLanguageToEnglishCommand = new RelayCommand(ChangeLanguageToEnglish);

            // Initialisation du gestionnaire de ressources pour les langues
            _resourceManager = new ResourceManager("EasySaveApp_WPF.Resources.Resources", Assembly.GetExecutingAssembly());
            _currentCulture = CultureInfo.CurrentUICulture;

            // Charger les ressources localisées
            LoadLocalizedResources();
        }

        private void LoadLocalizedResources()
        {
            // Charger les ressources localisées correspondant à la culture actuelle
            LocalizedStrings = _resourceManager.GetResourceSet(_currentCulture, true, true);
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

                // Charger les ressources localisées correspondant à la nouvelle culture
                LoadLocalizedResources();
            }
        }

        public string this[string key]
        {
            get
            {
                return _resourceManager.GetString(key, _currentCulture);
            }
        }
    }
}
