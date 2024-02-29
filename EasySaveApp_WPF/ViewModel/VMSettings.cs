using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace EasySaveApp_WPF.ViewModel
{
    public class ExtensionItem : VMBaseViewModel
    {
        private string _extension;
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; OnPropertyChanged(nameof(Extension)); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }
    }

    public class VMSettings : VMBaseViewModel
    {
        public ICommand SelectFormat { get; private set; }
        public ICommand AddCustomExtensionCommand { get; }
        public ICommand RemoveSelectedExtensionsCommand { get; }

        private bool _isXmlSelected;
        public bool IsXmlSelected
        {
            get { return _isXmlSelected; }
            set
            {
                _isXmlSelected = value;
                OnPropertyChanged("IsXmlSelected");
                if (value)
                {
                    OutputFormat = "xml";
                    IsJsonSelected = false;
                }
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
                if (value)
                {
                    OutputFormat = "json";
                    IsXmlSelected = false;
                }
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

        private ObservableCollection<ExtensionItem> _allowedExtensions = new ObservableCollection<ExtensionItem>();
        public ObservableCollection<ExtensionItem> AllowedExtensions
        {
            get { return _allowedExtensions; }
            set
            {
                _allowedExtensions = value;
                OnPropertyChanged(nameof(AllowedExtensions));
            }
        }

        private string _customExtension;
        public string CustomExtension
        {
            get { return _customExtension; }
            set
            {
                _customExtension = value;
                OnPropertyChanged(nameof(CustomExtension));
            }
        }
        
        public VMSettings()
        {
            // Initialize command bindings
            AddCustomExtensionCommand = new RelayCommand(AddCustomExtension);
            RemoveSelectedExtensionsCommand = new RelayCommand(RemoveSelectedExtensions);
            AllowedExtensions = new ObservableCollection<ExtensionItem>
        {
            new ExtensionItem { Extension = ".txt", IsSelected = true }
        };
            MaxFileSize = 100 * 1024; // Default max file size (100KB)
            SelectFormat = new RelayCommand(ConfirmFormat, CanConfirmFormat);
        }

        // Method to change language to English
        public static void TraductorEnglish()
        {
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri("/Resources/DictionaryEnglish.xaml", UriKind.RelativeOrAbsolute);
        }

        // Method to change language to French
        public static void TraductorFrench()
        {
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri("/Resources/DictionaryFrench.xaml", UriKind.RelativeOrAbsolute);
        }

        // Method to confirm output format selection
        private void ConfirmFormat(object parameter)
        {
            if (CanConfirmFormat(null))
            {
                MessageBox.Show($"Output format changed to {OutputFormat}.");
            }
            else
            {
                MessageBox.Show("Please select a format.");
            }
        }

        // Method to check if output format can be confirmed
        private bool CanConfirmFormat(object parameter)
        {
            return OutputFormat == "xml" || OutputFormat == "json";
        }

        // Method to add custom extension
        private void AddCustomExtension(object parameter)
        {
            if (!string.IsNullOrEmpty(CustomExtension))
            {
                string extension = CustomExtension.Trim();
                if (IsValidExtension(extension))
                {
                    if (!AllowedExtensions.Any(ext => ext.Extension == extension))
                    {
                        AllowedExtensions.Add(new ExtensionItem { Extension = extension, IsSelected = false });
                    }
                    else
                    {
                        MessageBox.Show("L'extension existe déjà dans la liste des extensions autorisées.");
                    }
                }
                else
                {
                    MessageBox.Show("Extension invalide. Veuillez saisir une extension au format correct (par exemple, '.txt').");
                }
            }
        }

        // Method to remove selected extensions
        private void RemoveSelectedExtensions(object parameter)
        {
            for (int i = AllowedExtensions.Count - 1; i >= 0; i--)
            {
                if (AllowedExtensions[i].IsSelected)
                {
                    AllowedExtensions.RemoveAt(i);
                }
            }
        }

        // Method to validate extension format
        private bool IsValidExtension(string extension)
        {
            Regex regex = new Regex(@"^\.[a-zA-Z0-9\-]+$");
            return regex.IsMatch(extension);
        }

        private int _maxFileSize;

        // Property for maximum file size in KB
        public int MaxFileSize
        {
            get { return _maxFileSize; }
            set
            {
                _maxFileSize = value;
                OnPropertyChanged(nameof(MaxFileSize));
            }
        }
    }
}
