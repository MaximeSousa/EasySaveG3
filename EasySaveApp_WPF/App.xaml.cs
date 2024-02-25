using System;
using System.Threading;
using System.Windows;

namespace EasySaveApp_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            const string mutexName = "AppInstance";

            try
            {
                _mutex = new Mutex(true, mutexName, out bool AppInstance);
                if (!AppInstance)
                {
                    MessageBox.Show("Application déja lancée");
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification de l'instance unique de l'application : {ex.Message}");
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
        }
    }
}
