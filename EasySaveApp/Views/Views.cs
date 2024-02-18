using System;
using System.IO;
using EasySaveApp.ViewsModel;
using System.Resources;

namespace EasySaveApp.Views
{
    class View
    {
        ViewModel Vm;

        public View()
        {
            Vm = new ViewModel();
        }

        public void ShowMenu()
        {
            Menu.ShowMainMenu(Vm);
        }
        public class Menu
        {
            // Variable pour stocker la langue sélectionnée
            private static string language = "English";
            private static ResourceManager resourceManager;

            private static void InitializeResourceManager()
            {
                string resourceFile = language == "Français" ? "Resource_fr" : "Resource_en";
                resourceManager = new ResourceManager($"EasySaveApp.Resources.{resourceFile}", typeof(Menu).Assembly);
            }
            public static void ShowMainMenu(ViewModel vm)
            {
                // Utilisez l'instance de ViewModel passée en paramètre
                InitializeResourceManager();
                bool exit = false;
                while (!exit)
                {
                    Console.WriteLine(resourceManager.GetString("MainMenuTitle"));
                    Console.WriteLine($"1. {resourceManager.GetString("CreateBackup")}");
                    Console.WriteLine($"2. {resourceManager.GetString("ExecuteBackup")}");
                    Console.WriteLine($"3. {resourceManager.GetString("ChangeBackup")}");
                    Console.WriteLine($"4. {resourceManager.GetString("DeleteBackup")}");
                    Console.WriteLine($"5. {resourceManager.GetString("ChangeLanguage")}");
                    Console.WriteLine($"6. {resourceManager.GetString("ChangeOutputFormat")}");
                    Console.WriteLine($"7. {resourceManager.GetString("Leave")}");

                    Console.WriteLine(resourceManager.GetString("ChooseAnOption"));
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            CreateBackup(vm);
                            break;
                        case "2":
                            ExecuteBackup(vm);
                            break;
                        case "3":
                            ChangeBackup(vm);
                            break;
                        case "4":
                            DeleteBackup(vm);
                            break;
                        case "5":
                            ChangeLanguage();
                            break;
                        case "6":
                            ChangeOutputFormat(vm);
                            break;
                        case "7":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine(resourceManager.GetString("InvalidOption"));
                            break;
                    }
                }
            }

            private static void LoadResources()
            {
                string resourceFile = language == "Français" ? "Resource_fr" : "Resource_en";
                resourceManager = new ResourceManager($"EasySaveApp.Resources.{resourceFile}", typeof(Menu).Assembly);
            }

            private static void CreateBackup(ViewModel vm)
            {
                try
                {
                    Console.Clear();
                    vm.CreateExecuteBackup();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            private static void ChangeBackup(ViewModel vm)
            {
                Console.Clear();
                vm.DisplayBackups();
                        
                try
                {
                    vm.ChangeBackup();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            private static void DeleteBackup(ViewModel vm)
            {
                Console.Clear();
                vm.DisplayBackups();
                        
                try
                {
                    vm.DeleteBackup();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                } 
            }

            private static void ExecuteBackup(ViewModel vm)
            {
                Console.WriteLine(resourceManager.GetString("ExecuteBackup"));
                Console.Clear();
                vm.DisplayBackups();
                Console.WriteLine("Enter the backup numbers to execute (e.g., '1', '1-3', '1;3'): ");
                string backupSelection = Console.ReadLine();
                string[] args = backupSelection.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    vm.ExeBackupJob(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            private static void ChangeLanguage()
            {
                // Si la langue actuelle est le français, changez-la en anglais et vice versa
                if (language == "Français")
                {
                    language = "English";
                }
                else
                {
                    language = "Français";
                }

                // Mettez à jour la variable de langue
                Console.Clear();
                Console.WriteLine($"Language changed to {language}.");

                // Chargez les ressources dans la nouvelle langue
                LoadResources();
            }
            private static void ChangeOutputFormat(ViewModel vm)
            {
                Console.WriteLine("Enter the output format (json or xml): ");
                string format = Console.ReadLine();
                if (format == "json" || format == "xml")
                {
                    vm.OutputFormat = format;
                    Console.Clear();
                    Console.WriteLine($"Output format changed to {format}.");
                }
                else
                {
                    Console.WriteLine("Invalid output format. Please enter either 'json' or 'xml'.");
                }
            }
        }
    }
}
