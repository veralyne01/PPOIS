using Minimarket;
using System.Security.Cryptography;

static class UI
{
    private const string Header =
        "-----------------------\n MINIMARKET SIMULATION\n-----------------------\n";

    public static void StartMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(Header);
            Console.Write("1. Open shop\n2. Exit\n> ");

            switch (Console.ReadLine())
            {
                case "1":
                    Shop shop = new Shop();
                    shop.Open();
                    Menu(shop);
                    return;             
                case "2":
                    return;
                default:
                    Console.WriteLine("Invalid Input! Please, try again");
                    Console.ReadLine();
                    break;              
            }
        }
    }

    static void Menu(Shop shop)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(Header);
            Console.Write("1. New shop\n2. Customers\n3. Inventory check\n4. Staff\n5. Sales history\n6. Return history\n7. Exit\n> ");

            switch (Console.ReadLine())
            {
                case "1":
                    shop = new Shop();
                    shop.Open();
                    Console.WriteLine("New shop successfully created! Press any button to continue\n");
                    Console.ReadLine();
                    break;
                case "2":
                    CustomerMenu(shop);
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("Invalid Input! Please, try again");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void CustomerMenu(Shop shop)
    {
        while (true)
        {
            Console.Clear();
            ShowCustomers(shop);
            Console.Write("\n1. Add random customer\n2. Add customer manually\n3. Back\n> ");

            switch (Console.ReadLine())
            {
                case "1":
                    AddRandomCustomer(shop);
                    break;
                case "2":
                    AddCustomerManually(shop);
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid Input! Please, try again");
                    Console.ReadLine();
                    break;
            }
        }
    }
    static void ShowCustomers(Shop shop)
    {
        Console.WriteLine("Id\t\tName\t\tStatus\n");
        foreach (var customer in shop.Customers)
        {
            Console.WriteLine($"{customer.id}\t\t{customer.name}\t\t{customer.status}");
        }
    }

    private static void AddCustomerManually(Shop shop)
    {
        Console.WriteLine("Enter customer name: ");
        string name;
        name = Console.ReadLine();
        Customer customer = new Customer(name);
        shop.AddCustomer(customer);
    }

    private static void AddRandomCustomer(Shop shop)
    {
        string name;
        name = RandomHelper.GetRandom<CustomerName>().ToString();
        Customer customer = new Customer(name);
        shop.AddCustomer(customer);
    } 
}