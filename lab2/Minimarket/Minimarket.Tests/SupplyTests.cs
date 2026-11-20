namespace Minimarket.Tests;

public class ProductCatalogTests
{
    [Fact]
    public void AddProduct_AppendsProduct()
    {
        ProductCatalog catalog = new();
        Product product = TestData.CreateProduct();

        catalog.AddProduct(product);

        Assert.Single(catalog.Products);
        Assert.Same(product, catalog.Products[0]);
    }

    [Fact]
    public void AddProduct_RejectsNull()
    {
        ProductCatalog catalog = new();
        Assert.Throws<ArgumentNullException>(() => catalog.AddProduct(null!));
    }

    [Fact]
    public void AddProductRejectsDuplicateBarcode()
    {
        ProductCatalog catalog = new();
        Product first = TestData.CreateProduct("First", barcode: "4811234567890");
        Product duplicate = TestData.CreateProduct("Duplicate", barcode: "4811234567890");

        catalog.AddProduct(first);

        Assert.Throws<InvalidProductDataException>(() => catalog.AddProduct(duplicate));
        Assert.Single(catalog.Products);
    }

    [Fact]
    public void FindByBarcodeReturnsRegisteredProduct()
    {
        ProductCatalog catalog = new();
        Product product = TestData.CreateProduct(barcode: "4811234567890");
        catalog.AddProduct(product);

        Assert.Same(product, catalog.FindByBarcode("4811234567890"));
        Assert.Null(catalog.FindByBarcode("0000000000000"));
    }

    [Fact]
    public void FindByNameIsCaseInsensitive()
    {
        ProductCatalog catalog = new();
        Product product = TestData.CreateProduct("Whole Milk");
        catalog.AddProduct(product);

        Assert.Same(product, catalog.FindByName("whole milk"));
        Assert.Null(catalog.FindByName("Bread"));
    }
}

public class SupplyTests
{
    [Fact]
    public void Supply_StoresSupplierDateAndBatches()
    {
        Supplier supplier = new("Supplier A");
        Supply supply = new(15, supplier, TestData.Today);
        ProductBatch batch = TestData.CreateBatch();

        supply.AddBatch(batch);

        Assert.Equal(15, supply.id);
        Assert.Same(supplier, supply.supplier);
        Assert.Equal(TestData.Today, supply.date);
        Assert.Single(supply.Batches);
        Assert.Same(batch, supply.Batches[0]);
    }

    [Fact]
    public void Supply_AddBatchRejectsNull()
    {
        Supply supply = new(1, new Supplier("Supplier"), TestData.Today);
        Assert.Throws<ArgumentNullException>(() => supply.AddBatch(null!));
    }

    [Fact]
    public void Supplier_RejectsEmptyNameAndNullSupply()
    {
        Assert.Throws<ArgumentException>(() => new Supplier(" "));

        Supplier supplier = new("Supplier");
        Assert.Throws<ArgumentNullException>(() => supplier.AddSupply(null!));
    }

    [Fact]
    public void Supplier_AddSupplyStoresItsOwnSupply()
    {
        Supplier supplier = new("Supplier A");
        Supply supply = new(1, supplier, TestData.Today);

        supplier.AddSupply(supply);

        Assert.Single(supplier.Supplies);
        Assert.Same(supply, supplier.Supplies[0]);
    }

    [Fact]
    public void Supplier_AddSupplyRejectsAnotherSuppliersSupply()
    {
        Supplier owner = new("Owner");
        Supplier another = new("Another");
        Supply supply = new(1, owner, TestData.Today);

        Assert.Throws<ArgumentException>(() => another.AddSupply(supply));
        Assert.Empty(another.Supplies);
    }
}

public class WarehouseTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_RejectsNonPositiveCapacity(int capacity)
    {
        Assert.Throws<ArgumentException>(() => new Warehouse(capacity));
    }

    [Fact]
    public void AddBatchUpdatesQuantityAndFreeCapacity()
    {
        Warehouse warehouse = new(20);
        ProductBatch batch = TestData.CreateBatch(quantity: 7);

        warehouse.AddBatch(batch);

        Assert.Equal(7, warehouse.CurrentQuantity);
        Assert.Equal(13, warehouse.FreeCapacity);
        Assert.Single(warehouse.Batches);
    }

    [Fact]
    public void AddBatchRejectsNull()
    {
        Warehouse warehouse = new(20);
        Assert.Throws<ArgumentNullException>(() => warehouse.AddBatch(null!));
    }

    [Fact]
    public void AddBatchRejectsExceedingCapacity()
    {
        Warehouse warehouse = new(5);
        Assert.Throws<InvalidOperationException>(() =>
            warehouse.AddBatch(TestData.CreateBatch(quantity: 6)));
        Assert.Equal(0, warehouse.CurrentQuantity);
    }

    [Fact]
    public void ReceiveSupplyCopiesBatchesAndDoesNotMutateSupply()
    {
        Product product = TestData.CreateProduct();
        Supplier supplier = new("Supplier");
        Supply supply = new(1, supplier, TestData.Today);
        ProductBatch sourceBatch = TestData.CreateBatch(product, 8, 5);
        supply.AddBatch(sourceBatch);
        Warehouse warehouse = new(20);

        warehouse.ReceiveSupply(supply);

        Assert.Equal(8, warehouse.CurrentQuantity);
        Assert.Equal(8, sourceBatch.quantity);
        Assert.NotSame(sourceBatch, warehouse.Batches[0]);
        Assert.Equal(sourceBatch.Id, warehouse.Batches[0].Id);
        Assert.Equal(8, warehouse.Batches[0].quantity);
    }

    [Fact]
    public void ReceiveSupplyRejectsNull()
    {
        Warehouse warehouse = new(10);
        Assert.Throws<ArgumentNullException>(() => warehouse.ReceiveSupply(null!));
    }

    [Fact]
    public void ReceiveSupplyChecksWholeSupplyBeforeAddingAnything()
    {
        Supplier supplier = new("Supplier");
        Supply supply = new(1, supplier, TestData.Today);
        supply.AddBatch(TestData.CreateBatch(quantity: 3));
        supply.AddBatch(TestData.CreateBatch(quantity: 4));
        Warehouse warehouse = new(6);

        Assert.Throws<InvalidOperationException>(() => warehouse.ReceiveSupply(supply));
        Assert.Equal(0, warehouse.CurrentQuantity);
        Assert.Equal(6, warehouse.FreeCapacity);
    }

    [Fact]
    public void GetAvailableQuantityIgnoresExpiredBatchesAndOtherProducts()
    {
        Product first = TestData.CreateProduct("Milk");
        Product second = TestData.CreateProduct("Bread", "Bread and cereal");
        Warehouse warehouse = new(100);
        warehouse.AddBatch(TestData.CreateBatch(first, 3, -1));
        warehouse.AddBatch(TestData.CreateBatch(first, 4, 0));
        warehouse.AddBatch(TestData.CreateBatch(first, 5, 2));
        warehouse.AddBatch(TestData.CreateBatch(second, 9, 2));

        Assert.Equal(9, warehouse.GetAvailableQuantity(first, TestData.Today));
        Assert.Equal(9, warehouse.GetAvailableQuantity(second, TestData.Today));
    }

    [Fact]
    public void RemoveExpiredBatchesRemovesOnlyEarlierDates()
    {
        Product product = TestData.CreateProduct();
        Warehouse warehouse = new(100);
        warehouse.AddBatch(TestData.CreateBatch(product, 2, -1));
        warehouse.AddBatch(TestData.CreateBatch(product, 3, 0));
        warehouse.AddBatch(TestData.CreateBatch(product, 4, 1));

        int removed = warehouse.RemoveExpiredBatches(TestData.Today);

        Assert.Equal(2, removed);
        Assert.Equal(7, warehouse.CurrentQuantity);
        Assert.Equal(2, warehouse.Batches.Count);
    }

    [Fact]
    public void TakeForTransferUsesFEFOAndPreservesBatchIdentity()
    {
        Product product = TestData.CreateProduct();
        Warehouse warehouse = new(100);
        ProductBatch later = TestData.CreateBatch(product, 10, 10);
        ProductBatch earlier = TestData.CreateBatch(product, 5, 2);
        warehouse.AddBatch(later);
        warehouse.AddBatch(earlier);

        List<ProductBatch> taken = warehouse.TakeForTransfer(
            product, 8, TestData.Today);

        Assert.Equal(8, taken.Sum(x => x.quantity));
        Assert.Equal(earlier.Id, taken[0].Id);
        Assert.Equal(5, taken[0].quantity);
        Assert.Equal(later.Id, taken[1].Id);
        Assert.Equal(3, taken[1].quantity);
        Assert.Equal(7, warehouse.CurrentQuantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void TakeForTransferRejectsNonPositiveQuantity(int quantity)
    {
        Warehouse warehouse = new(10);
        Assert.Throws<ArgumentException>(() =>
            warehouse.TakeForTransfer(TestData.CreateProduct(), quantity, TestData.Today));
    }

    [Fact]
    public void TakeForTransferRejectsInsufficientNonExpiredStock()
    {
        Product product = TestData.CreateProduct();
        Warehouse warehouse = new(10);
        warehouse.AddBatch(TestData.CreateBatch(product, 10, -1));

        Assert.Throws<InsufficientStockException>(() =>
            warehouse.TakeForTransfer(product, 1, TestData.Today));
        Assert.Equal(10, warehouse.CurrentQuantity);
    }

    [Fact]
    public void TakeForTransferRemovesEmptyBatches()
    {
        Product product = TestData.CreateProduct();
        Warehouse warehouse = new(10);
        warehouse.AddBatch(TestData.CreateBatch(product, 2));

        var taken = warehouse.TakeForTransfer(product, 2, TestData.Today);

        Assert.Single(taken);
        Assert.Empty(warehouse.Batches);
        Assert.Equal(0, warehouse.CurrentQuantity);
    }
}
