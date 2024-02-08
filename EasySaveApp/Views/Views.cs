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
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
