using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using EasySaveApp_WPF.ViewModel;
using System.Configuration;


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

        public VMSettings()
        {

            AddCustomExtensionCommand = new RelayCommand(AddCustomExtension);
            RemoveSelectedExtensionsCommand = new RelayCommand(RemoveSelectedExtensions);
            AddCustomPriorityExtensionCommand = new RelayCommand(AddCustomPriorityExtension);
            RemoveFromPriorityCommand = new RelayCommand(RemoveFromPriority);


            AllowedExtensions = new ObservableCollection<ExtensionItem>
        {
            new ExtensionItem { Extension = ".txt", IsSelected = true }
        };
            MaxFileSize = 100 * 1024;

            SelectFormat = new RelayCommand(ConfirmFormat, CanConfirmFormat);
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
        private bool CanConfirmFormat(object parameter)
        {
            return OutputFormat == "xml" || OutputFormat == "json";
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
        private ObservableCollection<ExtensionItem> _priorityExtensions = new ObservableCollection<ExtensionItem>();
        public ObservableCollection<ExtensionItem> PriorityExtensions
        {
            get { return _priorityExtensions; }
            set
            {
                _priorityExtensions = value;
                OnPropertyChanged(nameof(PriorityExtensions));
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

        private string _customPriorityExtension;
        public string CustomPriorityExtension
        {
            get { return _customPriorityExtension; }
            set
            {
                _customPriorityExtension = value;
                OnPropertyChanged(nameof(CustomPriorityExtension));
            }
        }

        public ICommand AddCustomExtensionCommand { get; }
        public ICommand RemoveSelectedExtensionsCommand { get; }
        public ICommand AddCustomPriorityExtensionCommand { get; }
        public ICommand RemoveFromPriorityCommand { get; }

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
        private void AddCustomPriorityExtension(object parameter)
        {
            if (!string.IsNullOrEmpty(CustomPriorityExtension))
            {
                string extension = CustomPriorityExtension.Trim();
                if (IsValidExtension(extension))
                {
                    if (!PriorityExtensions.Any(ext => ext.Extension == extension))
                    {
                        PriorityExtensions.Add(new ExtensionItem { Extension = extension, IsSelected = false });
                    }
                    else
                    {
                        MessageBox.Show("L'extension existe déjà dans la liste des extensions prioritaires.");
                    }
                }
                else
                {
                    MessageBox.Show("Extension invalide. Veuillez saisir une extension au format correct (par exemple, '.txt').");
                }
            }
        }
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
        private void RemoveFromPriority(object parameter)
        {
            for (int i = PriorityExtensions.Count - 1; i >= 0; i--)
            {
                if (PriorityExtensions[i].IsSelected)
                {
                    PriorityExtensions.RemoveAt(i);
                }
            }
        }

        private bool IsValidExtension(string extension)
        {
            Regex regex = new Regex(@"^\.[a-zA-Z0-9\-]+$");
            return regex.IsMatch(extension);
        }

        private int _maxFileSize;

        // propriété pour la taille maximale des fichiers en Ko
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
