using System.Reflection;

namespace Minimarket.Tests;

public class ShopTests
{
    [Fact]
    public void ConstructorInitializesDepartmentsStaffRegistersCatalogAndWarehouse()
    {
        Shop shop = new();

        Assert.Equal(5, shop.Departments.Count);
        Assert.Equal(6, shop.Employees.Count);
        Assert.Equal(4, shop.CashRegisters.Count);
        Assert.Equal(5, shop.Catalog.Products.Count);
        Assert.True(shop.Warehouse.CurrentQuantity > 0);
        Assert.All(shop.CashRegisters, register => Assert.False(register.IsOpen));
        Assert.All(shop.Departments, department => Assert.Equal(5, department.Shelves.Count));
    }

    [Fact]
    public void OpenOpensAllRegistersAndCannotBeCalledTwice()
    {
        Shop shop = new();

        shop.Open();

        Assert.True(shop.IsOpen);
        Assert.All(shop.CashRegisters, register => Assert.True(register.IsOpen));
        Assert.Throws<ShopAlreadyOpenedException>(() => shop.Open());
    }

    [Fact]
    public void AddCustomerAddsCustomerToShop()
    {
        Shop shop = new();
        Customer customer = new("Alice");

        shop.AddCustomer(customer);

        Assert.Single(shop.Customers);
        Assert.Same(customer, shop.Customers[0]);
    }


    [Fact]
    public void DisplayStoreInventoryReportsEmptyDepartment()
    {
        Shop shop = new();
        Department department = shop.Departments[0];
        foreach (Shelf shelf in department.Shelves)
            ((List<ProductBatch>)shelf.Batches).Clear();

        string output = TestData.CaptureOutput(shop.DisplayStoreInventory);

        Assert.Contains("No products in this department.", output);
    }

    [Fact]
    public void DisplayStoreInventoryGroupsSameProductAndExpirationAcrossShelves()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        Department department = shop.Departments.Single(
            d => d.name == product.category.CategoryName);
        DateOnly expiration = TestData.Today.AddDays(15);
        department.AddBatch(new ProductBatch(product, 60, expiration));

        string output = TestData.CaptureOutput(shop.DisplayStoreInventory);

        Assert.Contains("shelf(s): 1, 2", output);
    }

    [Fact]
    public void RequestProductsFromWarehouseRejectsWhenAutomaticSupplyExceedsWarehouseCapacity()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        Department department = shop.Departments.Single(
            d => d.name == product.category.CategoryName);
        int available = shop.Warehouse.GetAvailableQuantity(product, TestData.Today);
        if (available > 0)
            shop.Warehouse.TakeForTransfer(product, available, TestData.Today);

        // The requested deficit is deliberately larger than the whole warehouse capacity.
        Assert.Throws<InvalidOperationException>(() =>
            shop.RequestProductsFromWarehouse(product, 2_000));
    }

    [Fact]
    public void RequestProductsFromWarehouseCreatesAutomaticSupplierWhenNoneExists()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        int available = shop.Warehouse.GetAvailableQuantity(product, TestData.Today);
        if (available > 0)
            shop.Warehouse.TakeForTransfer(product, available, TestData.Today);

        List<Supplier> suppliers = (List<Supplier>)typeof(Shop)
            .GetField("suppliers", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(shop)!;
        suppliers.Clear();

        int placed = shop.RequestProductsFromWarehouse(product, 3);

        Assert.Equal(3, placed);
        Assert.Single(suppliers);
        Assert.Equal("Automatic Supplier", suppliers[0].name);
        Assert.Single(suppliers[0].Supplies);
    }

    [Fact]
    public void AddToHistoryKeepsOnlyLatestFiveReceipts()
    {
        Shop shop = new();
        MethodInfo method = typeof(Shop).GetMethod("AddToHistory", BindingFlags.Instance | BindingFlags.NonPublic)!;
        Product product = TestData.CreateProduct();

        List<Receipt> receipts = new();
        for (int i = 0; i < 6; i++)
        {
            var (_, receipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 1);
            receipts.Add(receipt);
            method.Invoke(shop, new object[] { receipt });
        }

        Assert.Equal(5, shop.RecentReceipts.Count);
        Assert.DoesNotContain(receipts[0], shop.RecentReceipts);
        Assert.Same(receipts[5], shop.RecentReceipts[^1]);
    }

    [Fact]
    public void MakeReturnDoesNothingWhenRecentReceiptsHaveNoReturnableItems()
    {
        Shop shop = new();
        Product product = TestData.CreateProduct();
        var (_, receipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 1);
        receipt.MarkReturned(product, 1);
        ((List<Receipt>)typeof(Shop)
            .GetField("recentReceipts", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(shop)!).Add(receipt);

        typeof(Shop).GetMethod("MakeReturn", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(shop, null);

        Assert.Empty(shop.Returns);
    }

    [Fact]
    public void NextStepHandlesProductWithoutMatchingDepartment()
    {
        Shop shop = new();
        shop.Open();
        Product product = TestData.CreateProduct("No department", "Unknown department", barcode: "9990000000002");
        shop.Catalog.AddProduct(product);
        Customer customer = new("Customer");
        customer.AddWish(product, 1);
        customer.timer = 1;
        shop.AddCustomer(customer);

        shop.NextStep();

        Assert.DoesNotContain(customer, shop.Customers);
        Assert.True(customer.cart.IsEmpty);
    }

    [Fact]
    public void NextStepReturnsCartGoodsWhenAllRegistersAreClosed()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        Department department = shop.Departments.Single(
            d => d.name == product.category.CategoryName);
        int before = department.GetQuantity(product);

        foreach (CashRegister register in shop.CashRegisters)
            TestData.SetPrivateField(register, "isOpen", false);

        Customer customer = new("Customer");
        customer.cart.AddBatch(TestData.CreateBatch(product, 2));
        customer.timer = 1;
        shop.AddCustomer(customer);

        shop.NextStep();

        Assert.DoesNotContain(customer, shop.Customers);
        Assert.True(customer.cart.IsEmpty);
        Assert.Equal(before + 2, department.GetQuantity(product));
    }

    [Fact]
    public void NextStepClearsCartWhenNoDepartmentCanAcceptReturnedGoods()
    {
        Shop shop = new();
        Product product = TestData.CreateProduct("No department", "Unknown", barcode: "9990000000003");
        foreach (CashRegister register in shop.CashRegisters)
            TestData.SetPrivateField(register, "isOpen", false);

        Customer customer = new("Customer");
        customer.cart.AddBatch(TestData.CreateBatch(product, 2));
        customer.timer = 1;
        shop.AddCustomer(customer);

        shop.NextStep();

        Assert.DoesNotContain(customer, shop.Customers);
        Assert.True(customer.cart.IsEmpty);
    }

    [Fact]
    public void NextStepCatchesFailureWhenReturnedItemsDoNotFitOnFullDepartment()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        Department department = shop.Departments.Single(
            d => d.name == product.category.CategoryName);
        department.AddBatch(TestData.CreateBatch(product, department.FreeCapacity));
        foreach (CashRegister register in shop.CashRegisters)
            TestData.SetPrivateField(register, "isOpen", false);

        Customer customer = new("Customer");
        customer.cart.AddBatch(TestData.CreateBatch(product, 1));
        customer.timer = 1;
        shop.AddCustomer(customer);

        shop.NextStep();

        Assert.DoesNotContain(customer, shop.Customers);
        Assert.True(customer.cart.IsEmpty);
    }

    [Fact]
    public void DisplayStoreInventoryMarksExpiredBatches()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        Department department = shop.Departments.Single(
            d => d.name == product.category.CategoryName);
        department.AddBatch(TestData.CreateBatch(product, 1, -1));

        string output = TestData.CaptureOutput(shop.DisplayStoreInventory);

        Assert.Contains("EXPIRED", output);
    }

    [Fact]
    public void DisplayStoreInventoryShowsDepartmentsProductsAndAvailability()
    {
        Shop shop = new();
        string output = TestData.CaptureOutput(shop.DisplayStoreInventory);

        Assert.Contains("Dairy", output);
        Assert.Contains("Milk 2.5%", output);
        Assert.Contains("quantity:", output);
        Assert.Contains("expiration:", output);
        Assert.Contains("AVAILABLE", output);
    }

    [Fact]
    public void RemoveExpiredProductsRemovesExpiredBatchesFromWarehouseAndStore()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        Department department = shop.Departments.Single(
            d => d.name == product.category.CategoryName);
        int before = shop.Warehouse.CurrentQuantity + department.CurrentQuantity;

        shop.Warehouse.AddBatch(TestData.CreateBatch(product, 2, -1));
        department.AddBatch(TestData.CreateBatch(product, 3, -1));

        int removed = shop.RemoveExpiredProducts();

        Assert.Equal(5, removed);
        Assert.Equal(before, shop.Warehouse.CurrentQuantity + department.CurrentQuantity);
        Assert.DoesNotContain(shop.Warehouse.Batches, b => b.expirationDate < TestData.Today);
        Assert.DoesNotContain(department.Shelves.SelectMany(s => s.Batches), b => b.expirationDate < TestData.Today);
    }

    [Fact]
    public void RequestProductsFromWarehouseMovesRequestedQuantityIntoDepartment()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        Department department = shop.Departments.Single(
            d => d.name == product.category.CategoryName);
        int before = department.GetQuantity(product);

        int placed = shop.RequestProductsFromWarehouse(product, 5);

        Assert.Equal(5, placed);
        Assert.Equal(before + 5, department.GetQuantity(product));
    }

    [Fact]
    public void RequestProductsFromWarehouseAutomaticallyReplenishesEmptyStock()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        Department department = shop.Departments.Single(
            d => d.name == product.category.CategoryName);
        int beforeInStore = department.GetQuantity(product);
        int freeCapacity = department.FreeCapacity;

        // Take every warehouse batch of this product out before requesting it.
        int availableInWarehouse = shop.Warehouse.GetAvailableQuantity(product, TestData.Today);
        if (availableInWarehouse > 0)
        {
            shop.Warehouse.TakeForTransfer(product, availableInWarehouse, TestData.Today);
        }

        int placed = shop.RequestProductsFromWarehouse(product, 6);

        Assert.Equal(6, placed);
        Assert.Equal(beforeInStore + 6, department.GetQuantity(product));
        Assert.True(department.FreeCapacity < freeCapacity);
    }

    [Fact]
    public void RequestProductsFromWarehouseReturnsExcessToWarehouseWhenDepartmentIsFull()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        Department department = shop.Departments.Single(
            d => d.name == product.category.CategoryName);

        int remainingCapacity = department.FreeCapacity;
        department.AddBatch(TestData.CreateBatch(product, remainingCapacity, 20));
        Assert.Equal(0, department.FreeCapacity);

        int placed = shop.RequestProductsFromWarehouse(product, 7);

        Assert.Equal(0, placed);
        Assert.Equal(250, department.CurrentQuantity);
        Assert.Equal(0, department.FreeCapacity);
    }

    [Fact]
    public void RequestProductsFromWarehouseResolvesCatalogInstanceByBarcode()
    {
        Shop shop = new();
        Product registered = shop.Catalog.Products[0];
        Product detachedCopy = new(
            registered.Name,
            registered.category,
            registered.manufacturer,
            registered.price,
            registered.barcode,
            registered.shelfLifeDays);

        int placed = shop.RequestProductsFromWarehouse(detachedCopy, 1);

        Assert.Equal(1, placed);
        Assert.Same(registered, shop.StockMovements.Last(movement =>
            movement.Type == SupplyOperations.Transfer).Product);
    }

    [Fact]
    public void RequestProductsFromWarehouseRejectsNullProduct()
    {
        Shop shop = new();
        Assert.Throws<ArgumentNullException>(() => shop.RequestProductsFromWarehouse(null!, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RequestProductsFromWarehouseRejectsNonPositiveQuantity(int quantity)
    {
        Shop shop = new();
        Assert.Throws<ArgumentException>(() =>
            shop.RequestProductsFromWarehouse(shop.Catalog.Products[0], quantity));
    }

    [Fact]
    public void RequestProductsFromWarehouseRejectsProductOutsideCatalog()
    {
        Shop shop = new();
        Product product = TestData.CreateProduct("Unknown", barcode: "9990000000004");

        Assert.Throws<ProductNotFoundException>(() =>
            shop.RequestProductsFromWarehouse(product, 1));
    }

    [Fact]
    public void RequestProductsFromWarehouseRejectsCatalogProductWithoutDepartment()
    {
        Shop shop = new();
        Product product = TestData.CreateProduct("Unknown", "Unknown department", barcode: "9990000000001");
        shop.Catalog.AddProduct(product);

        Assert.Throws<ProductNotFoundException>(() =>
            shop.RequestProductsFromWarehouse(product, 1));
    }

    [Fact]
    public void NextStepRemovesCustomerWhoHasNoWishes()
    {
        Shop shop = new();
        shop.Open();
        Customer customer = new("No wishes");
        shop.AddCustomer(customer);
        customer.timer = 1;

        shop.NextStep();

        Assert.DoesNotContain(customer, shop.Customers);
        Assert.All(shop.CashRegisters, register => Assert.Equal(0, register.QueueLength));
    }

    [Fact]
    public void NextStepRemovesCustomerWhenWantedProductIsUnavailable()
    {
        Shop shop = new();
        shop.Open();
        Product product = shop.Catalog.Products[0];
        Department department = shop.Departments.Single(
            d => d.name == product.category.CategoryName);
        int available = department.GetAvailableQuantity(product, TestData.Today);
        if (available > 0)
            department.TakeProduct(product, available, TestData.Today);

        Customer customer = new("Missing product");
        customer.AddWish(product, 1);
        customer.timer = 1;
        shop.AddCustomer(customer);

        shop.NextStep();

        Assert.DoesNotContain(customer, shop.Customers);
        Assert.True(customer.cart.IsEmpty);
    }

    [Fact]
    public void NextStepCompletesPurchaseForCustomerWithEnoughCardFunds()
    {
        Shop shop = new();
        shop.Open();
        Product product = shop.Catalog.Products[0];
        Customer customer = TestData.CreateCardOnlyCustomer();
        customer.card!.Deposit(100_000);
        customer.AddWish(product, 1);
        customer.timer = 1;
        shop.AddCustomer(customer);

        for (int i = 0; i < 10 && shop.RecentReceipts.Count == 0; i++)
            shop.NextStep();

        Assert.Single(shop.RecentReceipts);
        Assert.Single(shop.CashRegisters.SelectMany(r => r.Purchases));
        Assert.DoesNotContain(customer, shop.Customers);
    }

    [Fact]
    public void NextStepReturnsProductsToShelfAfterFailedPayment()
    {
        Shop shop = new();
        shop.Open();
        Product product = TestData.CreateProduct("Very expensive", "Dairy", 1_000_000, 30, "9990000000010");
        shop.Catalog.AddProduct(product);
        Department department = shop.Departments.Single(d => d.name == "Dairy");
        department.AddBatch(TestData.CreateBatch(product, 2, 20));

        Customer customer = TestData.CreateCardOnlyCustomer();
        customer.AddWish(product, 1);
        customer.timer = 1;
        shop.AddCustomer(customer);

        for (int i = 0; i < 10 && shop.Customers.Contains(customer); i++)
            shop.NextStep();

        Assert.DoesNotContain(customer, shop.Customers);
        Assert.True(customer.cart.IsEmpty);
        Assert.Equal(2, department.GetQuantity(product));
        Assert.Empty(shop.RecentReceipts);
    }

    [Fact]
    public void MakeReturnDoesNothingWhenThereAreNoReceipts()
    {
        Shop shop = new();
        MethodInfo method = typeof(Shop).GetMethod("MakeReturn", BindingFlags.Instance | BindingFlags.NonPublic)!;

        method.Invoke(shop, null);

        Assert.Empty(shop.Returns);
    }

    [Fact]
    public void MakeReturnCanCreateReturnFromRecentReceipt()
    {
        Shop shop = new();
        Product product = shop.Catalog.Products[0];
        var (customer, receipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 2);
        customer.card!.Deposit(100_000);

        List<Receipt> recentReceipts = (List<Receipt>)typeof(Shop)
            .GetField("recentReceipts", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(shop)!;
        recentReceipts.Add(receipt);

        MethodInfo method = typeof(Shop).GetMethod("MakeReturn", BindingFlags.Instance | BindingFlags.NonPublic)!;
        method.Invoke(shop, null);

        Assert.Single(shop.Returns);
        Assert.Same(receipt, shop.Returns[0].receipt);
        Assert.InRange(shop.Returns[0].products[product], 1, 2);
    }
}

public class ConsoleUiTests
{
    private static string RunUi(string input)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using StringReader reader = new(input);
        using StringWriter writer = new();
        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);
            global::UI.StartMenu();
            return writer.ToString();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void StartMenuCanExitImmediately()
    {
        string output = RunUi("2\n");
        Assert.Contains("MINIMARKET SIMULATION", output);
        Assert.Contains("2. Exit", output);
    }

    [Fact]
    public void StartMenuHandlesInvalidChoiceAndThenExits()
    {
        string output = RunUi("invalid\n\n2\n");
        Assert.Contains("Invalid Input!", output);
    }

    [Fact]
    public void StartMenuCanOpenShopAndExitMainMenu()
    {
        string output = RunUi("1\n7\n");
        Assert.Contains("Shop successfully opened!", output);
        Assert.Contains("Inventory check", output);
    }

    [Fact]
    public void InventoryMenuCanDisplayInventoryAndReturn()
    {
        string output = RunUi("1\n3\n1\n\n4\n7\n");
        Assert.Contains("INVENTORY", output);
        Assert.Contains("Dairy", output);
        Assert.Contains("AVAILABLE", output);
    }

    [Fact]
    public void InventoryMenuCanRemoveExpiredProducts()
    {
        string output = RunUi("1\n3\n2\n\n4\n7\n");
        Assert.Contains("Expired products removed:", output);
    }

    [Fact]
    public void InventoryMenuHandlesInvalidProductIndex()
    {
        string output = RunUi("1\n3\n3\n0\n\n4\n7\n");
        Assert.Contains("Invalid product number!", output);
    }

    [Fact]
    public void InventoryMenuHandlesInvalidQuantity()
    {
        string output = RunUi("1\n3\n3\n1\n0\n\n4\n7\n");
        Assert.Contains("Invalid quantity!", output);
    }

    [Fact]
    public void InventoryMenuCanRequestProductFromWarehouse()
    {
        string output = RunUi("1\n3\n3\n1\n1\n\n4\n7\n");
        Assert.Contains("Requested:", output);
        Assert.Contains("Placed in store:", output);
    }

    [Fact]
    public void CustomerMenuCanAddRandomCustomer()
    {
        string output = RunUi("1\n2\n1\n5\n7\n");
        Assert.Contains("Add random customer", output);
        Assert.Contains("Name:", output);
    }

    [Fact]
    public void CustomerMenuCanAddCustomerManuallyAndWishList()
    {
        string output = RunUi("1\n2\n2\nTest User\n1\n2\n\n5\n7\n");
        Assert.Contains("Test User", output);
        Assert.Contains("Wish list:", output);
    }

    [Fact]
    public void CustomerMenuRejectsEmptyManualName()
    {
        string output = RunUi("1\n2\n2\n\n\n5\n7\n");
        Assert.Contains("Name cannot be empty!", output);
    }

    [Fact]
    public void CustomerMenuCanAdvanceOneStep()
    {
        string output = RunUi("1\n2\n3\n\n5\n7\n");
        Assert.Contains("Next step", output);
    }

    [Fact]
    public void CustomerMenuRejectsInvalidNumberOfSteps()
    {
        string output = RunUi("1\n2\n4\n0\n\n5\n7\n");
        Assert.Contains("Invalid Input!", output);
    }

    [Fact]
    public void CustomerMenuCanRunSeveralSteps()
    {
        string output = RunUi("1\n2\n4\n2\n\n5\n7\n");
        Assert.Contains("--- Step 1 ---", output);
        Assert.Contains("--- Step 2 ---", output);
    }

    [Fact]
    public void MainMenuCanDisplayStaffAndExitNestedMenu()
    {
        string output = RunUi("1\n4\n\n7\n7\n");
        Assert.Contains("Inventory Manager", output);
        Assert.Contains("Return Manager", output);
    }

    [Fact]
    public void MainMenuCanDisplaySalesAndReturnHistory()
    {
        string sales = RunUi("1\n5\n\n7\n");
        Assert.Contains("Register", sales);
        Assert.Contains("no purchases yet", sales);

        string returns = RunUi("1\n6\n\n7\n");
        Assert.Contains("No returns yet", returns);
    }

    [Fact]
    public void MainMenuCanCreateANewShop()
    {
        string output = RunUi("1\n1\n\n7\n");

        Assert.Contains("New shop successfully created!", output);
        Assert.Contains("Shop successfully opened!", output);
    }

    [Fact]
    public void SalesHistoryDisplaysStoredPurchase()
    {
        Shop shop = new();
        CashRegister register = shop.CashRegisters[0];
        Product product = TestData.CreateProduct("History product", price: 125);
        var (_, receipt) = TestData.CreateReceipt(
            product, cardPayment: true, purchasedQuantity: 2);

        List<Purchase> purchases = (List<Purchase>)typeof(CashRegister)
            .GetField("purchases", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(register)!;
        purchases.Add(receipt.purchase);

        string output = InvokeUiMethod("ShowSalesHistory", shop);

        Assert.Contains("History product x2", output);
        Assert.Contains("total: 250", output);
        Assert.Contains("payment: Card", output);
    }

    [Fact]
    public void ReturnHistoryDisplaysStoredReturn()
    {
        Shop shop = new();
        Product product = TestData.CreateProduct("Returned product", price: 175);
        var (_, receipt) = TestData.CreateReceipt(
            product, cardPayment: true, purchasedQuantity: 2);
        Return returned = new(receipt, product, 1, 175, "Defective");

        List<Return> returns = (List<Return>)typeof(Shop)
            .GetField("returns", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(shop)!;
        returns.Add(returned);

        string output = InvokeUiMethod("ShowReturnHistory", shop);

        Assert.Contains("Returned product x1", output);
        Assert.Contains("refunded: 175", output);
        Assert.Contains("Defective", output);
    }

    private static string InvokeUiMethod(string methodName, Shop shop)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using StringReader reader = new("\n");
        using StringWriter writer = new();

        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);
            MethodInfo method = typeof(global::UI).GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException(
                    $"UI method {methodName} was not found.");
            method.Invoke(null, new object[] { shop });
            return writer.ToString();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void MainMenuHandlesInvalidChoice()
    {
        string output = RunUi("1\ninvalid\n\n7\n");
        Assert.Contains("Invalid Input!", output);
    }
}
