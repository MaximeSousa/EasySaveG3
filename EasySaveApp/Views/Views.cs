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

            public static void InitializeResourceManager()
            {
                string resourceFile = language == "Français" ? "Resource_fr" : "Resource_en";
                resourceManager = new ResourceManager($"EasySaveApp.Resources.{resourceFile}", typeof(Menu).Assembly);

            }

            public void InitializeResourceManagerViewsModels()
            {
                InitializeResourceManager();

            }

            public static void ShowMainMenu(ViewModel vm)
            {
                // Utilisez l'instance de ViewModel passée en paramètre
                InitializeResourceManager();
                bool exit = false;
                while (!exit)
                {
                    Console.WriteLine(resourceManager.GetString("Main Menu Title"));
                    Console.WriteLine($"1. {resourceManager.GetString("Create Backup")}");
                    Console.WriteLine($"2. {resourceManager.GetString("Execute Backup")}");
                    Console.WriteLine($"3. {resourceManager.GetString("Change Backup")}");
                    Console.WriteLine($"4. {resourceManager.GetString("Delete Backup")}");
                    Console.WriteLine($"5. {resourceManager.GetString("Change Language")}");
                    Console.WriteLine($"6. {resourceManager.GetString("Leave")}");

                    Console.WriteLine(resourceManager.GetString("Choose An Option"));
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
                            exit = true;
                            break;
                        default:
                            Console.WriteLine(resourceManager.GetString("Invalid Option"));
                            break;
                    }
                }
            }

            public string getLanguage() { return language; }

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
                Console.WriteLine(resourceManager.GetString("Execute Backup"));
                Console.Clear();
                vm.DisplayBackups();
                Console.WriteLine(resourceManager.GetString("Enter the backup numbers to execute (e.g., '1', '1-3', '1;3'): "));
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

            public string getTraductor(string word)
            {
                return resourceManager.GetString(word);
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
                Console.WriteLine($"{resourceManager.GetString($"Language changed to {language}.")}");

                // Chargez les ressources dans la nouvelle langue
                InitializeResourceManager();

            }
        }
    }
}
