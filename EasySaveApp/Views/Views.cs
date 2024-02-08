using System;
using System.IO;
using EasySaveApp.ViewsModel;

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
                Console.WriteLine("3. Change a backup");
                Console.WriteLine("4. Delete a backup");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        try
                        {
                            Console.Clear();
                            Vm.CreateExecuteBackup();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    case "2":
                        Console.Clear();
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
                    case "3":
                        Console.Clear();
                        Vm.DisplayBackups();
                        
                        try
                        {
                            Vm.ChangeBackup();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "4":
                        Console.Clear();
                        Vm.DisplayBackups();
                        try
                        {
                            Vm.DeleteBackup();
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
