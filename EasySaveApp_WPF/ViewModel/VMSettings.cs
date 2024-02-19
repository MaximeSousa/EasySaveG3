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
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri("Resources/DictionaryEnglish.xaml", UriKind.RelativeOrAbsolute);
        }
        public void TraductorFrench()
        {
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri("Resources/DictionaryFrench.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
