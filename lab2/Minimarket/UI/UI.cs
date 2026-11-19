using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    static class UI
    {
        public static void StartMenu()
        {
            Console.Clear();
            Console.WriteLine("-----------------------\n MINIMARKET SIMULATION\n-----------------------\n");
            Console.WriteLine("1. Open shop\n2. Exit\n> ");
            string? input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Shop shop = new Shop();
                    Shop.OpenShop(shop);
                    Menu();
                    break;
                case "2":
                    return;
                default:
                    Console.WriteLine("Invalid Input! Please, try again");
                    Console.ReadLine();
                    StartMenu();
                    break;
            }
        }

        static void Menu()
        {
            Console.Clear();
            Console.WriteLine("-----------------------\n MINIMARKET SIMULATION\n-----------------------\n");
            Console.WriteLine("1. New shop\n2. Customers\n3. Inventory check\n4. Staff\n5. Sales history\n6. Return history\n7. Inventory check\n8. Exit\n> > ");
            string? input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    break;
            }
        }
    }
}
