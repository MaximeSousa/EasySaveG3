using System;
using System.IO;
using EasySaveApp.ViewsModel;
using EasySaveApp.Models;
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
        public class Menu
        {
            // Variable pour stocker la langue sélectionnée
            private static string language = "English";
            private static ResourceManager resourceManager; // = new ResourceManager("EasySaveApp.Resources.Strings", Assembly.GetExecutingAssembly());


            public static void ShowMainMenu()
            {
                // Charger les ressources en fonction de la langue sélectionnée
                LoadResources();

                bool exit = false;
                while (!exit)
                {
                    Console.WriteLine(resourceManager.GetString("MainMenuTitle"));
                    Console.WriteLine($"1. {resourceManager.GetString("CreateBackup")}");
                    Console.WriteLine($"2. {resourceManager.GetString("ExecuteBackup")}");
                    Console.WriteLine($"3. {resourceManager.GetString("ChangeLanguage")}");
                    Console.WriteLine($"4. {resourceManager.GetString("Leave")}");

                    Console.WriteLine(resourceManager.GetString("ChooseAnOption"));
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            CreateBackupMenu();
                            break;
                        case "2":
                            ExecuteBackup();
                            break;
                        case "3":
                            ChangeLanguage();
                            break;
                        case "4":
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

            private static void CreateBackupMenu()
            {
                Console.Clear();
                Console.WriteLine(resourceManager.GetString("CreateBackup"));
                Console.WriteLine($"1. {resourceManager.GetString("CreateSingle")}");
                Console.WriteLine($"2. {resourceManager.GetString("CreateMultiple")}");
                Console.WriteLine(resourceManager.GetString("ChooseAnOption"));
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateSingleBackup();
                        break;
                    case "2":
                        CreateMultipleBackup();
                        break;
                    default:
                        Console.WriteLine(resourceManager.GetString("InvalidOption"));
                        break;
                }
            }

            private static void CreateSingleBackup()
            {
                Console.WriteLine(resourceManager.GetString("CreateSingle"));
                Console.WriteLine($"1. {resourceManager.GetString("Full")}");
                Console.WriteLine($"2. {resourceManager.GetString("Differential")}");
                Console.WriteLine(resourceManager.GetString("ChooseAnOption"));

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine(resourceManager.GetString("FullBackupMethod"));
                        break;
                    case "2":
                        Console.WriteLine(resourceManager.GetString("DifferentialBackupMethod"));
                        break;
                    default:
                        Console.WriteLine(resourceManager.GetString("InvalidOption"));
                        break;
                }
            }

            private static void CreateMultipleBackup()
            {
                Console.WriteLine(resourceManager.GetString("CreateMultiple"));
                Console.WriteLine($"1. {resourceManager.GetString("Full")}");
                Console.WriteLine($"2. {resourceManager.GetString("Differential")}");
                Console.WriteLine(resourceManager.GetString("ChooseAnOption"));

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine(resourceManager.GetString("FullBackupMethod"));
                        break;
                    case "2":
                        Console.WriteLine(resourceManager.GetString("DifferentialBackupMethod"));
                        break;
                    default:
                        Console.WriteLine(resourceManager.GetString("InvalidOption"));
                        break;
                }
            }

            private static void ExecuteBackup()
            {
                Console.WriteLine(resourceManager.GetString("ExecuteBackup"));
                // Implémentez la logique pour exécuter la sauvegarde
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
                Console.WriteLine($"Language changed to {language}.");

                // Chargez les ressources dans la nouvelle langue
                LoadResources();
            }
        }
    }
}
