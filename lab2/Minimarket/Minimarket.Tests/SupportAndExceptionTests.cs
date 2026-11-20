using System.Reflection;

namespace Minimarket.Tests;

public class ExceptionTests
{
    [Fact]
    public void ProjectExceptionsAreRealExceptionsWithUsefulMessages()
    {
        Exception[] exceptions =
        {
            new ShopAlreadyOpenedException(),
            new InsufficientFundsException(),
            new InsufficientFundsException("custom funds message"),
            new InsufficientStockException(),
            new InvalidChangeException(),
            new InvalidChangeException("custom change message"),
            new InvalidPaymentException(),
            new InvalidPaymentException("custom payment message"),
            new InvalidPhoneNumberException(),
            new InvalidProductDataException(),
            new InvalidReturnException(),
            new InvalidReturnException("custom return message"),
            new ProductNotFoundException(),
            new ProductNotFoundException("custom product message"),
            new ExpiredProductException(),
            new ExpiredProductException("custom expiration message"),
            new InvalidBarcodeException(),
            new InvalidSupplyException(),
            new InvalidSupplyException("custom supply message"),
            new ReceiptNotFoundException(),
            new SupplierNotFoundException()
        };

        Assert.All(exceptions, exception =>
            Assert.False(string.IsNullOrWhiteSpace(exception.Message)));
        Assert.Equal("custom funds message", exceptions[2].Message);
        Assert.Equal("custom payment message", exceptions[7].Message);
    }

    [Fact]
    public void InvalidBarcodeExceptionIsThrownForNonDigitBarcode()
    {
        Assert.Throws<InvalidBarcodeException>(() =>
            new Product(
                "Milk",
                new ProductCategory("Dairy"),
                new Manufacturer("Maker", Country.Belarus),
                100,
                "48A123",
                10));
    }

    [Fact]
    public void InvalidSupplyExceptionIsThrownForExpiredIncomingBatch()
    {
        Product product = TestData.CreateProduct();
        Supplier supplier = new("Supplier");
        Supply supply = new(10, supplier, TestData.Today);

        Assert.Throws<InvalidSupplyException>(() =>
            supply.AddBatch(TestData.CreateBatch(product, 2, -1)));
    }

    [Fact]
    public void SupplierNotFoundExceptionIsThrownForUnknownSupplier()
    {
        Shop shop = new();
        Assert.Throws<SupplierNotFoundException>(() => shop.FindSupplier("Unknown supplier"));
    }

    [Fact]
    public void ReceiptNotFoundExceptionIsThrownForUnknownReceipt()
    {
        Shop shop = new();
        Assert.Throws<ReceiptNotFoundException>(() =>
            shop.ProcessReturn(-100, TestData.CreateProduct(), 1, "Defective"));
    }
}

public class PhoneNumberTests
{
    private static object CreatePhoneNumber(int operatorCode, int number)
    {
        ConstructorInfo constructor = typeof(PhoneNumber).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(int), typeof(int) },
            modifiers: null)
            ?? throw new InvalidOperationException("PhoneNumber constructor not found.");

        try
        {
            return constructor.Invoke(new object[] { operatorCode, number });
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }

    [Theory]
    [InlineData(25)]
    [InlineData(29)]
    [InlineData(33)]
    [InlineData(44)]
    public void PhoneNumberAcceptsSupportedOperatorCodes(int operatorCode)
    {
        Assert.NotNull(CreatePhoneNumber(operatorCode, 1234567));
    }

    [Fact]
    public void PhoneNumberRejectsUnsupportedOperatorCode()
    {
        Assert.Throws<InvalidPhoneNumberException>(() => CreatePhoneNumber(21, 1234567));
    }

    [Fact]
    public void PhoneNumberRejectsMoreThanSevenDigits()
    {
        Assert.Throws<InvalidPhoneNumberException>(() => CreatePhoneNumber(29, 12345678));
    }

    [Fact]
    public void PhoneNumberRejectsNegativeNumber()
    {
        Assert.Throws<InvalidPhoneNumberException>(() => CreatePhoneNumber(29, -1));
    }

    [Fact]
    public void CustomerCanStorePhoneNumber()
    {
        PhoneNumber phone = PhoneNumber.Create(29, 1234567);
        Customer customer = new("Phone customer", phone);

        Assert.Same(phone, customer.phoneNumber);
        Assert.Equal("+375 (29) 1234567", customer.phoneNumber!.ToString());
        Assert.Equal(TestData.Today, customer.DateOfRegistration);
    }
}


public class EmployeeModelTests
{
    [Fact]
    public void EmployeeRejectsEmptyName()
    {
        Assert.Throws<ArgumentException>(() => new Cashier(" "));
    }

    [Fact]
    public void EmployeeStoresNameAndUniqueNumber()
    {
        Cashier first = new("Alice");
        Cashier second = new("Bob");

        Assert.Equal("Alice", first.Name);
        Assert.Equal("Bob", second.Name);
        Assert.NotEqual(first.number, second.number);
    }
}

public class InternalModelTests
{
    [Fact]
    public void LabelReadsBarcodeExpiryAndBatchId()
    {
        ProductBatch batch = TestData.CreateBatch(quantity: 4, daysUntilExpiration: 8);

        Assert.Equal(batch.product.barcode, batch.label.Barcode);
        Assert.Equal(batch.expirationDate, batch.label.ExpirationDate);
        Assert.Equal(batch.Id, batch.label.BatchId);
    }

    [Fact]
    public void LocationCanRepresentWarehouseAndShelf()
    {
        Department department = new("Dairy");
        Location warehouse = Location.Warehouse;
        Location shelf = new(department, department.Shelves[0]);

        Assert.Equal("Warehouse", warehouse.Name);
        Assert.Same(department, shelf.Department);
        Assert.Same(department.Shelves[0], shelf.Shelf);
        Assert.True(department.Shelves[0].Id > 0);
        Assert.NotEqual(department.Shelves[0].Id, department.Shelves[1].Id);
        Assert.Contains("Dairy", shelf.ToString());
    }

    [Fact]
    public void LocationRejectsShelfFromAnotherDepartment()
    {
        Department first = new("Dairy");
        Department second = new("Sweets");

        Assert.Throws<ArgumentException>(() =>
            new Location(first, second.Shelves[0]));
    }

    [Fact]
    public void ProductPriceHistoryStoresInitialAndChangedPrices()
    {
        Product product = TestData.CreateProduct(price: 100);

        product.ChangePrice(125);
        product.ChangePrice(150);
        product.ChangePrice(150);

        Assert.Equal(new[] { 100, 125, 150 }, product.PriceHistory.Prices);
        Assert.Equal(150, product.PriceHistory.CurrentPrice);
        Assert.Same(product, product.PriceHistory.Product);
    }

    [Fact]
    public void InventoryCheckAndReportSummarizeCountsAndExpiredProducts()
    {
        Product product = TestData.CreateProduct();
        InventoryCheck check = new(TestData.Today, new Cashier("Inventory cashier"));
        check.AddItem(new InventoryCheckItem(
            product, 5, 7, Location.Warehouse, TestData.Today.AddDays(3)));
        check.AddItem(new InventoryCheckItem(
            product, 4, 4, new Location("Dairy, shelf 1"), TestData.Today.AddDays(-1)));

        check.Complete();
        InventoryReport report = new(check);

        Assert.True(check.status);
        Assert.Equal(9, report.TotalExpected);
        Assert.Equal(11, report.TotalActual);
        Assert.Equal(2, report.Difference);
        Assert.Equal(4, report.ExpiredQuantity);
        Assert.Equal(1, report.DiscrepancyCount);
        Assert.False(report.IsBalanced);
    }

    [Fact]
    public void InventoryCheckCannotBeModifiedAfterCompletion()
    {
        InventoryCheck check = new(TestData.Today, new Cashier("Cashier"));
        check.Complete();

        Assert.Throws<InvalidOperationException>(() =>
            check.AddItem(new InventoryCheckItem(
                TestData.CreateProduct(), 1, 1, Location.Warehouse, TestData.Today)));
    }

    [Fact]
    public void StockMovementStoresSourceDestinationAndQuantity()
    {
        Product product = TestData.CreateProduct();
        Location from = Location.Warehouse;
        Location to = new("Dairy");
        StockMovement movement = new(
            product, 6, SupplyOperations.Transfer, TestData.Today, from, to);

        Assert.Same(product, movement.Product);
        Assert.Equal(6, movement.Quantity);
        Assert.Equal(SupplyOperations.Transfer, movement.Type);
        Assert.Same(from, movement.From);
        Assert.Same(to, movement.To);
    }

    [Fact]
    public void ShopReplenishmentOrderStoresTransferResult()
    {
        Product product = TestData.CreateProduct();
        Warehouse warehouse = new(30);
        Department department = new("Dairy");
        warehouse.AddBatch(TestData.CreateBatch(product, 5));
        List<ProductBatch> batches = warehouse.TakeForTransfer(product, 5, TestData.Today);
        StockTransfer transfer = new(
            warehouse, department, product, batches, 5, TestData.Today);
        transfer.Execute();

        ShopReplenishmentOrder order = new(product, 5);
        order.Complete(transfer);

        Assert.True(order.isCompleted);
        Assert.True(order.isSatisfied);
        Assert.Equal(5, order.placedAmount);
        Assert.Equal(0, order.returnedAmount);
    }

    [Fact]
    public void WarehouseReplenishmentRequestCanBeApprovedAndRequiresSupplier()
    {
        Supplier supplier = new("Supplier A");
        WarehouseReplenishmentRequest request = new(
            supplier, TestData.CreateProduct(), 12, TestData.Today);

        Assert.False(request.isApproved);
        request.Approve();
        Assert.True(request.isApproved);
        request.Reject();
        Assert.False(request.isApproved);

        Assert.Throws<SupplierNotFoundException>(() =>
            new WarehouseReplenishmentRequest(
                null, TestData.CreateProduct(), 1, TestData.Today));
    }

    [Fact]
    public void InventoryManagerCreatesReportForWarehouseAndStoreShelves()
    {
        Product product = TestData.CreateProduct();
        Warehouse warehouse = new(30);
        warehouse.AddBatch(TestData.CreateBatch(product, 5));
        Department department = new("Dairy");
        department.AddBatch(TestData.CreateBatch(product, 3));
        InventoryManager manager = new("Inventory Manager");

        InventoryReport report = manager.PerformInventoryCheck(
            warehouse, new[] { department }, TestData.Today);

        Assert.True(report.Check.status);
        Assert.Equal(8, report.TotalExpected);
        Assert.Equal(8, report.TotalActual);
        Assert.True(report.IsBalanced);
        Assert.Equal(2, report.Items.Count);
    }

    [Fact]
    public void RandomHelperReturnsDefinedEnumValues()
    {
        for (int i = 0; i < 100; i++)
        {
            Country country = RandomHelper.GetRandom<Country>();
            Assert.True(Enum.IsDefined(country));
        }
    }
}
