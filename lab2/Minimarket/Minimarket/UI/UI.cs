using Minimarket;
using System.Security.Cryptography;
using System.IO;

static class UI
{
    private const string Header =
        "-----------------------\n MINIMARKET SIMULATION\n-----------------------\n";
    private static void ClearConsole()
    {
        if (Console.IsOutputRedirected)
            return;

        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
            
        }
        catch (InvalidOperationException)
        {
           
        }
        catch (PlatformNotSupportedException)
        {
         
        }
        catch (System.ComponentModel.Win32Exception)
        {
           
        }
    }

    public static void StartMenu()
    {
        while (true)
        {
            ClearConsole();
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
            ClearConsole();
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
                case "3":
                    InventoryMenu(shop);
                    break;
                case "4":
                    ShowStaff(shop);
                    break;
                case "5":
                    ShowSalesHistory(shop);
                    break;
                case "6":
                    ShowReturnHistory(shop);
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

    static void InventoryMenu(Shop shop)
    {
        while (true)
        {
            ClearConsole();
            Console.WriteLine("========== INVENTORY ==========");
            Console.Write("1. View products in departments\n2. Remove expired products\n3. Request products from warehouse\n4. Back\n5. Run inventory report\n> ");

            switch (Console.ReadLine())
            {
                case "1":
                    ClearConsole();
                    shop.DisplayStoreInventory();
                    Console.WriteLine("\nPress any button to continue");
                    Console.ReadLine();
                    break;
                case "2":
                    ClearConsole();
                    shop.RemoveExpiredProducts();
                    Console.WriteLine("\nPress any button to continue");
                    Console.ReadLine();
                    break;
                case "3":
                    RequestProductsFromWarehouse(shop);
                    break;
                case "4":
                    return;
                case "5":
                    ClearConsole();
                    InventoryReport report = shop.PerformInventoryCheck();
                    Console.WriteLine("========== INVENTORY REPORT ==========");
                    Console.WriteLine($"Date: {report.Date}");
                    Console.WriteLine($"Expected quantity: {report.TotalExpected}");
                    Console.WriteLine($"Actual quantity: {report.TotalActual}");
                    Console.WriteLine($"Difference: {report.Difference}");
                    Console.WriteLine($"Expired quantity: {report.ExpiredQuantity}");
                    Console.WriteLine($"Discrepancies: {report.DiscrepancyCount}");
                    Console.WriteLine($"Balanced: {report.IsBalanced}");
                    Console.WriteLine("\nPress any button to continue");
                    Console.ReadLine();
                    break;
                default:
                    Console.WriteLine("Invalid Input! Please, try again");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private static void RequestProductsFromWarehouse(Shop shop)
    {
        ClearConsole();
        var products = shop.Catalog.Products;

        Console.WriteLine("Products:");
        for (int i = 0; i < products.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {products[i].Name} ({products[i].category.CategoryName})");
        }

        Console.Write("\nEnter product number\n> ");
        if (!int.TryParse(Console.ReadLine(), out int number) ||
            number < 1 || number > products.Count)
        {
            Console.WriteLine("Invalid product number!");
            Console.ReadLine();
            return;
        }

        Console.Write("Enter quantity\n> ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity!");
            Console.ReadLine();
            return;
        }

        try
        {
            shop.RequestProductsFromWarehouse(products[number - 1], quantity);
        }
        catch (Exception ex) when (
            ex is ProductNotFoundException ||
            ex is InsufficientStockException ||
            ex is InvalidOperationException ||
            ex is ArgumentException)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
        }

        Console.WriteLine("\nPress any button to continue");
        Console.ReadLine();
    }

    static void CustomerMenu(Shop shop)
    {
        while (true)
        {
            ClearConsole();
            ShowCustomers(shop);
            Console.Write("\n1. Add random customer\n2. Add customer manually\n3. Next step\n4. Run several steps\n5. Back\n> ");

            switch (Console.ReadLine())
            {
                case "1":
                    AddRandomCustomer(shop);
                    break;
                case "2":
                    AddCustomerManually(shop);
                    break;
                case "3":
                    ClearConsole();
                    shop.NextStep();
                    Console.WriteLine("\nPress any button to continue");
                    Console.ReadLine();
                    break;
                case "4":
                    RunSteps(shop);
                    break;
                case "5":
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
        foreach (var customer in shop.Customers)
        {
            Console.WriteLine($"Name:\t{customer.name}");
            Console.WriteLine($"Id:\t{customer.id}");
            Console.WriteLine($"Status:\t{customer.status}");
            Console.WriteLine($"Registered:\t{customer.DateOfRegistration:dd.MM.yyyy}");
            if (customer.hasBankCard)
            {
                Console.WriteLine($"Bank card: {customer.card.number}, date of registration: {customer.card.dateOfRegistration}, sum: {customer.card.money}");
            }
            if (customer.hasWallet)
            {
                Console.WriteLine($"Amount in cash: {customer.wallet.sum}\t");
            }
            Console.WriteLine($"Loyality card: {customer.loyalityCard.number}, date of registration: {customer.loyalityCard.dateOfRegistration}, bonuses: {customer.loyalityCard.bonuses}");
            if (customer.HasWishes)
            {
                Console.WriteLine("Wish list: " + string.Join(", ",
                    customer.WishList.Select(wish => $"{wish.Key.Name} x{wish.Value}")));
            }
            if (!customer.cart.IsEmpty)
            {
                Console.WriteLine("Cart: " + string.Join(", ",
                    customer.cart.ProductList.Select(item => $"{item.product.Name} x{item.number}"))
                    + $" (sum: {customer.cart.CalculateSum()})");
            }
            Console.WriteLine("\n***\n");
        }
    }

    private static void AddCustomerManually(Shop shop)
    {
        Console.WriteLine("Enter customer name: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty! Press any button to continue");
            Console.ReadLine();
            return;
        }
        Customer customer = new Customer(name);
        EnterWishList(shop, customer);
        shop.AddCustomer(customer);
    }

    private static void AddRandomCustomer(Shop shop)
    {
        string name;
        name = RandomHelper.GetRandom<CustomerName>().ToString();
        Customer customer = new Customer(name);
        customer.GenerateWishList(shop.Catalog);
        shop.AddCustomer(customer);
    }

    private static void ShowStaff(Shop shop)
    {
        ClearConsole();
        Console.WriteLine("Employee\t\tNumber\n");
        foreach (var employee in shop.Employees)
        {
            Console.WriteLine($"{employee.Name}\t\t{employee.number}");
        }
        Console.WriteLine("\nPress any button to continue");
        Console.ReadLine();
        Menu(shop);
    }
    private static void EnterWishList(Shop shop, Customer customer)
    {
        var products = shop.Catalog.Products;
        while (true)
        {
            Console.WriteLine("\nProducts:");
            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {products[i].Name}\t{products[i].price}");
            }
            Console.Write("Enter product number (empty line to finish)\n> ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                return;
            if (!int.TryParse(input, out int number) || number < 1 || number > products.Count)
            {
                Console.WriteLine("Invalid Input! Please, try again");
                continue;
            }
            Console.Write("Enter quantity\n> ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 1)
            {
                Console.WriteLine("Invalid Input! Please, try again");
                continue;
            }
            customer.AddWish(products[number - 1], quantity);
        }
    }
    private static void RunSteps(Shop shop)
    {
        Console.Write("How many steps? \n> ");
        if (!int.TryParse(Console.ReadLine(), out int steps) || steps < 1)
        {
            Console.WriteLine("Invalid Input! Please, try again");
            Console.ReadLine();
            return;
        }
        ClearConsole();
        for (int i = 1; i <= steps; i++)
        {
            Console.WriteLine($"--- Step {i} ---");
            shop.NextStep();
        }
        Console.WriteLine("\nPress any button to continue");
        Console.ReadLine();
    }
    private static void ShowSalesHistory(Shop shop)
    {
        ClearConsole();
        foreach (CashRegister register in shop.CashRegisters)
        {
            Console.WriteLine($"Register {register.Id}:");
            if (register.Purchases.Count == 0)
            {
                Console.WriteLine("  no purchases yet");
            }
            foreach (Purchase purchase in register.Purchases)
            {
                Console.WriteLine(
                    $"  #{purchase.id} {purchase.date}, customer: {purchase.customer.name}, " +
                    $"cashier: {purchase.cashier.Name}, total: {purchase.total}, " +
                    $"payment: {purchase.payment.method}, bonuses spent/accrued: " +
                    $"{purchase.bonusesSpent}/{purchase.bonusesAccrued}");
                foreach (PurchaseItem item in purchase.Items)
                {
                    Console.WriteLine($"      {item.product.Name} x{item.number} ({item.price} each)");
                }
            }
        }
        Console.WriteLine("\nLast customers:");
        foreach (Receipt receipt in shop.RecentReceipts)
        {
            Console.WriteLine($"  receipt #{receipt.id}: {receipt.purchase.customer.name}, sum: {receipt.sum}");
        }
        Console.WriteLine("\nPress any button to continue");
        Console.ReadLine();
    }
    private static void ShowReturnHistory(Shop shop)
    {
        ClearConsole();
        if (shop.Returns.Count == 0)
        {
            Console.WriteLine("No returns yet");
        }
        foreach (Return ret in shop.Returns)
        {
            string products = string.Join(", ",
                ret.products.Select(pair => $"{pair.Key.Name} x{pair.Value}"));
            Console.WriteLine(
                $"#{ret.id} {ret.date}, receipt #{ret.receipt.id}, customer: {ret.receipt.purchase.customer.name}, " +
                $"{products}, refunded: {ret.sum} ({ret.receipt.payment.method}), reason: {ret.reason}");
        }
        Console.WriteLine("\nPress any button to continue");
        Console.ReadLine();
    }
}
