using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using EasySaveApp_WPF.ViewModel;

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
        public VMSettings()
        {
            AddCustomExtensionCommand = new RelayCommand(AddCustomExtension);
            RemoveSelectedExtensionsCommand = new RelayCommand(RemoveSelectedExtensions);

            AllowedExtensions = new ObservableCollection<ExtensionItem>
        {
            new ExtensionItem { Extension = ".docx", IsSelected = true }
        };
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

        public ICommand AddCustomExtensionCommand { get; }
        public ICommand RemoveSelectedExtensionsCommand { get; }

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

        private bool IsValidExtension(string extension)
        {
            Regex regex = new Regex(@"^\.[a-zA-Z0-9\-]+$");
            return regex.IsMatch(extension);
        }
    }
}
