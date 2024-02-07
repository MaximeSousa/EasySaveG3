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
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Vm.CreateExecuteBackup();
                        break;
                    case "2":
                        Console.WriteLine("Prochainement");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
