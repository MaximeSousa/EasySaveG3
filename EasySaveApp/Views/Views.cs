using System;
using System.IO;
using EasySaveApp.ViewsModel;
using EasySaveApp.Models;
namespace EasySaveApp.Views
{
    class View
    {
        ViewModel Vm;

        public View()
        {
            Vm = new ViewModel();
        }
        public void Menu()
        {
            while (true)
            {
                Console.WriteLine("1. Create a backup");
                Console.WriteLine("2. Execute a backup");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        try
                        {
                            Vm.CreateExecuteBackup();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    case "2":
                        Vm.DisplayBackups();
                        Console.WriteLine("Enter the backup numbers to execute (e.g., '1', '1-3', '1;3'): ");
                        string backupSelection = Console.ReadLine();
                        string[] args = backupSelection.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        try
                        {
                            Vm.ExeBacjupJob(args);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
