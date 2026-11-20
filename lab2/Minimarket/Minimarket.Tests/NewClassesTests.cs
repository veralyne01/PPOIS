namespace Minimarket.Tests;

public class DiscountTests
{
    [Fact]
    public void DiscountIsActiveOnBothBoundaryDates()
    {
        Product product = TestData.CreateProduct(price: 250);
        Discount discount = new(
            "Weekend", product, 20, TestData.Today, TestData.Today.AddDays(2));

        Assert.True(discount.IsActive(TestData.Today));
        Assert.True(discount.IsActive(TestData.Today.AddDays(2)));
        Assert.False(discount.IsActive(TestData.Today.AddDays(3)));
    }

    [Fact]
    public void DiscountCalculatesPerUnitAmountOnlyForItsProductAndPeriod()
    {
        Product product = TestData.CreateProduct(price: 250);
        Product another = TestData.CreateProduct("Another", price: 100);
        Discount discount = new(
            "Test discount", product, 20, TestData.Today, TestData.Today.AddDays(5));

        Assert.Equal(50, discount.CalculateDiscountPerUnit(TestData.Today));
        Assert.False(discount.AppliesTo(another, TestData.Today));
        Assert.Equal(0, discount.CalculateDiscountPerUnit(TestData.Today.AddDays(6)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(100)]
    [InlineData(101)]
    public void DiscountRejectsInvalidPercent(int percent)
    {
        Product product = TestData.CreateProduct();
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Discount("Invalid", product, percent, TestData.Today, TestData.Today.AddDays(1)));
    }

    [Fact]
    public void DiscountRejectsInvalidPeriodAndEmptyName()
    {
        Product product = TestData.CreateProduct();

        Assert.Throws<ArgumentException>(() =>
            new Discount(" ", product, 10, TestData.Today, TestData.Today.AddDays(1)));
        Assert.Throws<ArgumentException>(() =>
            new Discount("Broken period", product, 10, TestData.Today.AddDays(2), TestData.Today));
    }

    [Fact]
    public void DiscountCalculatorRejectsNullDiscountAndCart()
    {
        DiscountCalculator calculator = new();
        Assert.Throws<ArgumentNullException>(() => calculator.AddDiscount(null!));
        Assert.Throws<ArgumentNullException>(() => calculator.CalculateTotalDiscount(null!, TestData.Today));
    }

    [Fact]
    public void DiscountCalculatorUsesLargestApplicableDiscountWithoutStacking()
    {
        Product product = TestData.CreateProduct(price: 300);
        DiscountCalculator calculator = new();
        calculator.AddDiscount(new Discount(
            "Ten percent", product, 10, TestData.Today, TestData.Today.AddDays(2)));
        calculator.AddDiscount(new Discount(
            "Twenty percent", product, 20, TestData.Today, TestData.Today.AddDays(2)));

        Assert.Equal(60, calculator.CalculateDiscountPerUnit(product, TestData.Today));
        Assert.Equal(0, calculator.CalculateDiscountPerUnit(product, TestData.Today.AddDays(3)));
    }

    [Fact]
    public void DiscountCalculatorCalculatesTotalDiscountForCart()
    {
        Product product = TestData.CreateProduct(price: 200);
        ShoppingCart cart = new();
        cart.AddBatch(TestData.CreateBatch(product, 3));
        DiscountCalculator calculator = new();
        calculator.AddDiscount(new Discount(
            "Quarter off", product, 25, TestData.Today, TestData.Today.AddDays(1)));

        Assert.Equal(150, calculator.CalculateTotalDiscount(cart, TestData.Today));
    }
}

public class StockTransferTests
{
    [Fact]
    public void ExecuteMovesBatchFromWarehouseToDepartmentAndRecordsMovement()
    {
        Product product = TestData.CreateProduct();
        Warehouse warehouse = new(100);
        Department department = new("Dairy");
        warehouse.AddBatch(TestData.CreateBatch(product, 12));
        List<ProductBatch> taken = warehouse.TakeForTransfer(product, 8, TestData.Today);

        StockTransfer transfer = new(
            warehouse, department, product, taken, 8, TestData.Today);
        int placed = transfer.Execute();

        Assert.Equal(8, placed);
        Assert.Equal(8, department.GetQuantity(product));
        Assert.Equal(4, warehouse.CurrentQuantity);
        Assert.True(transfer.IsCompleted);
        Assert.Equal(0, transfer.ReturnedQuantity);
        Assert.Single(transfer.Movements);
        Assert.Equal(SupplyOperations.Transfer, transfer.Movements[0].Type);
    }

    [Fact]
    public void ExecuteReturnsPartThatDoesNotFitBackToWarehouse()
    {
        Product product = TestData.CreateProduct();
        Warehouse warehouse = new(100);
        Department department = new("Dairy");
        department.AddBatch(TestData.CreateBatch(product, 248));
        warehouse.AddBatch(TestData.CreateBatch(product, 10));
        List<ProductBatch> taken = warehouse.TakeForTransfer(product, 10, TestData.Today);

        StockTransfer transfer = new(
            warehouse, department, product, taken, 10, TestData.Today);
        int placed = transfer.Execute();

        Assert.Equal(2, placed);
        Assert.Equal(8, transfer.ReturnedQuantity);
        Assert.Equal(8, warehouse.CurrentQuantity);
        Assert.Equal(250, department.CurrentQuantity);
        Assert.Equal(2, transfer.Movements.Single().Quantity);
    }

    [Fact]
    public void ExecuteRejectsExpiredBatch()
    {
        Product product = TestData.CreateProduct();
        Warehouse warehouse = new(10);
        Department department = new("Dairy");
        ProductBatch expired = TestData.CreateBatch(product, 2, -1);
        StockTransfer transfer = new(
            warehouse, department, product, new[] { expired }, 2, TestData.Today);

        Assert.Throws<ExpiredProductException>(() => transfer.Execute());
        Assert.Equal(0, department.CurrentQuantity);
        Assert.False(transfer.IsCompleted);
    }

    [Fact]
    public void ConstructorRejectsBatchForAnotherProduct()
    {
        Product product = TestData.CreateProduct();
        Product other = TestData.CreateProduct("Other");
        Warehouse warehouse = new(20);
        Department department = new("Dairy");

        Assert.Throws<InvalidOperationException>(() =>
            new StockTransfer(warehouse, department, product,
                new[] { TestData.CreateBatch(other, 2) }, 2, TestData.Today));
    }

    [Fact]
    public void ConstructorRejectsWrongDepartmentBeforeChangingBatches()
    {
        Product product = TestData.CreateProduct(category: "Dairy");
        Warehouse warehouse = new(20);
        Department department = new("Sweets");
        ProductBatch batch = TestData.CreateBatch(product, 2);

        Assert.Throws<InvalidOperationException>(() =>
            new StockTransfer(warehouse, department, product,
                new[] { batch }, 2, TestData.Today));
        Assert.Equal(2, batch.quantity);
    }

    [Fact]
    public void ExecuteCannotBeRepeated()
    {
        Product product = TestData.CreateProduct();
        Warehouse warehouse = new(10);
        Department department = new("Dairy");
        warehouse.AddBatch(TestData.CreateBatch(product, 1));
        List<ProductBatch> taken = warehouse.TakeForTransfer(product, 1, TestData.Today);
        StockTransfer transfer = new(warehouse, department, product, taken, 1, TestData.Today);
        transfer.Execute();

        Assert.Throws<InvalidOperationException>(() => transfer.Execute());
    }
}

public class ScheduledReturnTests
{
    [Fact]
    public void ScheduledReturnBecomesDueAtConfiguredStep()
    {
        Product product = TestData.CreateProduct();
        var (_, receipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 3);
        ScheduledReturn scheduled = new(receipt, product, 2, "Defective", 5);

        Assert.False(scheduled.IsDue(4));
        Assert.True(scheduled.IsDue(5));
        Assert.False(scheduled.IsProcessed);
    }

    [Fact]
    public void ScheduledReturnCanBePostponedAndCompleted()
    {
        Product product = TestData.CreateProduct();
        var (_, receipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 3);
        ScheduledReturn scheduled = new(receipt, product, 1, "Changed my mind", 2);

        scheduled.Postpone(3);
        Assert.False(scheduled.IsDue(4));
        Assert.True(scheduled.IsDue(5));

        scheduled.MarkProcessed();
        Assert.True(scheduled.IsProcessed);
        Assert.False(scheduled.IsDue(10));
        Assert.Throws<InvalidOperationException>(() => scheduled.MarkProcessed());
    }

    [Fact]
    public void ScheduledReturnRejectsEmptyReasonAndNegativeDueStep()
    {
        Product product = TestData.CreateProduct();
        var (_, receipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 2);

        Assert.Throws<ArgumentException>(() =>
            new ScheduledReturn(receipt, product, 1, " ", 1));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ScheduledReturn(receipt, product, 1, "Defective", -1));
    }

    [Fact]
    public void ScheduledReturnRejectsQuantityAboveReturnableAmount()
    {
        Product product = TestData.CreateProduct();
        var (_, receipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 1);

        Assert.Throws<InvalidReturnException>(() =>
            new ScheduledReturn(receipt, product, 2, "Defective", 1));
    }
}

public class ShopIntegrationForNewClassesTests
{
    [Fact]
    public void ShopRegistersInitialStockMovementAndReplenishmentOrder()
    {
        Shop shop = new();

        Assert.NotEmpty(shop.StockMovements);
        Assert.NotEmpty(shop.ReplenishmentOrders);
        Assert.All(shop.ReplenishmentOrders, order => Assert.True(order.isCompleted));
        Assert.Contains(shop.StockMovements, movement => movement.Type == SupplyOperations.Transfer);
        Assert.Contains(shop.StockMovements, movement => movement.Type == SupplyOperations.Arrival);
    }

    [Fact]
    public void InventoryCheckCanBeRequestedThroughShop()
    {
        Shop shop = new();

        InventoryReport report = shop.PerformInventoryCheck();

        Assert.True(report.Check.status);
        Assert.Single(shop.InventoryChecks);
        Assert.True(report.IsBalanced);
        Assert.True(report.TotalActual > 0);
    }

    [Fact]
    public void CashRegisterAppliesConfiguredProductDiscountToPurchase()
    {
        Shop shop = new();
        shop.Open();

        Product chocolate = shop.Catalog.Products.Single(product => product.Name == "Chocolate");
        Customer customer = TestData.CreateCardOnlyCustomer();
        customer.card!.Deposit(100_000);
        customer.loyalityCard!.bonuses = 0;
        customer.cart.AddBatch(TestData.CreateBatch(chocolate, 2));

        CashRegister register = shop.CashRegisters[0];
        register.Enqueue(customer);

        Receipt? receipt = null;
        for (int i = 0; i < 10 && receipt == null; i++)
            register.NextStep(out receipt);

        Assert.NotNull(receipt);
        Assert.Equal(495 * 2, receipt!.sum);
        Assert.Equal(55, receipt.Items.Single().discount);
        Assert.Equal(990, receipt.payment.sum);
    }
}
