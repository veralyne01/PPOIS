namespace Minimarket.Tests;

public class ProductTests
{
    [Fact]
    public void Product_StoresConstructorValues()
    {
        ProductCategory category = new("Dairy");
        Manufacturer manufacturer = new("Manufacturer", Country.Belarus);
        Product product = new("Milk", category, manufacturer, 350, "481123", 10);

        Assert.Equal("Milk", product.Name);
        Assert.Same(category, product.category);
        Assert.Same(manufacturer, product.manufacturer);
        Assert.Equal(350, product.price);
        Assert.Equal("481123", product.barcode);
        Assert.Equal(10, product.shelfLifeDays);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(100, 0)]
    [InlineData(100, -1)]
    public void Product_RejectsInvalidPriceOrShelfLife(int price, int shelfLife)
    {
        Assert.Throws<InvalidProductDataException>(() =>
            TestData.CreateProduct(price: price, shelfLifeDays: shelfLife));
    }

    [Fact]
    public void Product_ChangePriceUpdatesPrice()
    {
        Product product = TestData.CreateProduct(price: 100);

        product.ChangePrice(250);

        Assert.Equal(250, product.price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Product_ChangePriceRejectsNonPositiveValue(int price)
    {
        Product product = TestData.CreateProduct();
        Assert.Throws<InvalidProductDataException>(() => product.ChangePrice(price));
    }

    [Fact]
    public void ManufacturerAndCategory_ExposeValues()
    {
        ProductCategory category = new("Sweets");
        Manufacturer manufacturer = new("Kommunarka", Country.Belarus);

        Assert.Equal("Sweets", category.CategoryName);
        Assert.Equal("Kommunarka", manufacturer.ManufacturerName);
        Assert.Equal(Country.Belarus, manufacturer.country);
    }
}

public class ProductBatchTests
{
    [Fact]
    public void Constructor_StoresProductQuantityAndExpiration()
    {
        Product product = TestData.CreateProduct();
        DateOnly expiration = TestData.Today.AddDays(5);

        ProductBatch batch = new(product, 12, expiration);

        Assert.True(batch.Id > 0);
        Assert.Same(product, batch.product);
        Assert.Equal(12, batch.quantity);
        Assert.Equal(expiration, batch.expirationDate);
    }

    [Fact]
    public void Constructor_RejectsNullProduct()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ProductBatch(null!, 1, TestData.Today));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_RejectsNonPositiveQuantity(int quantity)
    {
        Assert.Throws<ArgumentException>(() =>
            new ProductBatch(TestData.CreateProduct(), quantity, TestData.Today));
    }

    [Fact]
    public void Take_CreatesPartOfSameBatchAndDecreasesOriginal()
    {
        ProductBatch batch = TestData.CreateBatch(quantity: 10);

        ProductBatch part = batch.Take(4);

        Assert.Equal(6, batch.quantity);
        Assert.Equal(4, part.quantity);
        Assert.Equal(batch.Id, part.Id);
        Assert.Same(batch.product, part.product);
        Assert.Equal(batch.expirationDate, part.expirationDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    public void Take_RejectsInvalidAmount(int amount)
    {
        ProductBatch batch = TestData.CreateBatch(quantity: 5);
        Assert.Throws<ArgumentException>(() => batch.Take(amount));
        Assert.Equal(5, batch.quantity);
    }

    [Fact]
    public void Take_AllQuantityLeavesOriginalEmpty()
    {
        ProductBatch batch = TestData.CreateBatch(quantity: 3);
        ProductBatch part = batch.Take(3);

        Assert.Equal(0, batch.quantity);
        Assert.Equal(3, part.quantity);
    }

    [Fact]
    public void RemoveQuantity_DecreasesQuantity()
    {
        ProductBatch batch = TestData.CreateBatch(quantity: 8);
        batch.RemoveQuantity(3);
        Assert.Equal(5, batch.quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    public void RemoveQuantity_RejectsInvalidAmount(int amount)
    {
        ProductBatch batch = TestData.CreateBatch(quantity: 5);
        Assert.Throws<ArgumentException>(() => batch.RemoveQuantity(amount));
        Assert.Equal(5, batch.quantity);
    }

    [Fact]
    public void Copy_PreservesIdAndValuesWithoutChangingOriginal()
    {
        ProductBatch batch = TestData.CreateBatch(quantity: 7, daysUntilExpiration: 4);
        ProductBatch copy = batch.Copy();

        Assert.NotSame(batch, copy);
        Assert.Equal(batch.Id, copy.Id);
        Assert.Same(batch.product, copy.product);
        Assert.Equal(batch.quantity, copy.quantity);
        Assert.Equal(batch.expirationDate, copy.expirationDate);
        Assert.Equal(7, batch.quantity);
    }
}

public class ShelfTests
{
    [Fact]
    public void Shelf_AddBatchUpdatesCurrentAndFreeCapacity()
    {
        Shelf shelf = new(1);
        ProductBatch batch = TestData.CreateBatch(quantity: 12);

        shelf.AddBatch(batch);

        Assert.Equal(12, shelf.currentQuantity);
        Assert.Equal(38, shelf.FreeCapacity);
        Assert.Single(shelf.Batches);
    }

    [Fact]
    public void Shelf_RejectsBatchOverCapacity()
    {
        Shelf shelf = new(1);
        Assert.Throws<InvalidOperationException>(() =>
            shelf.AddBatch(TestData.CreateBatch(quantity: 51)));
    }

    [Fact]
    public void Shelf_AvailableQuantityIgnoresExpiredBatches()
    {
        Product product = TestData.CreateProduct();
        Shelf shelf = new(1);
        shelf.AddBatch(TestData.CreateBatch(product, 4, -1));
        shelf.AddBatch(TestData.CreateBatch(product, 7, 0));
        shelf.AddBatch(TestData.CreateBatch(product, 3, 2));

        Assert.Equal(10, shelf.GetAvailableQuantity(product, TestData.Today));
        Assert.Equal(0, shelf.GetAvailableQuantity(TestData.CreateProduct("Other"), TestData.Today));
    }

    [Fact]
    public void Shelf_RemoveExpiredBatchesRemovesOnlyPastDates()
    {
        Product product = TestData.CreateProduct();
        Shelf shelf = new(1);
        shelf.AddBatch(TestData.CreateBatch(product, 3, -1));
        shelf.AddBatch(TestData.CreateBatch(product, 4, 0));
        shelf.AddBatch(TestData.CreateBatch(product, 5, 1));

        int removed = shelf.RemoveExpiredBatches(TestData.Today);

        Assert.Equal(3, removed);
        Assert.Equal(9, shelf.currentQuantity);
        Assert.Equal(2, shelf.Batches.Count);
    }

    [Fact]
    public void Shelf_TakeProductUsesEarliestExpirationFirst()
    {
        Product product = TestData.CreateProduct();
        Shelf shelf = new(1);
        ProductBatch later = TestData.CreateBatch(product, 5, 8);
        ProductBatch earlier = TestData.CreateBatch(product, 3, 2);
        shelf.AddBatch(later);
        shelf.AddBatch(earlier);

        List<ProductBatch> taken = shelf.TakeProduct(product, 4, TestData.Today);

        Assert.Equal(4, taken.Sum(x => x.quantity));
        Assert.Equal(3, taken[0].quantity);
        Assert.Equal(earlier.expirationDate, taken[0].expirationDate);
        Assert.Equal(1, taken[1].quantity);
        Assert.Equal(later.expirationDate, taken[1].expirationDate);
        Assert.Equal(4, shelf.currentQuantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void Shelf_TakeProductRejectsNonPositiveQuantity(int quantity)
    {
        Shelf shelf = new(1);
        Assert.Throws<ArgumentException>(() =>
            shelf.TakeProduct(TestData.CreateProduct(), quantity, TestData.Today));
    }

    [Fact]
    public void Shelf_TakeProductThrowsWhenStockIsInsufficient()
    {
        Shelf shelf = new(1);
        shelf.AddBatch(TestData.CreateBatch(quantity: 2));

        Assert.Throws<InsufficientStockException>(() =>
            shelf.TakeProduct(shelf.Batches[0].product, 3, TestData.Today));
    }
}

public class DepartmentTests
{
    [Fact]
    public void Department_CreatesFiveShelves()
    {
        Department department = new("Dairy");

        Assert.Equal("Dairy", department.name);
        Assert.Equal(5, department.Shelves.Count);
        Assert.Equal(250, department.FreeCapacity);
        Assert.Equal(0, department.CurrentQuantity);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Department_RejectsEmptyName(string name)
    {
        Assert.Throws<ArgumentException>(() => new Department(name));
    }

    [Fact]
    public void Department_AddBatchDistributesAcrossShelves()
    {
        Department department = new("Dairy");
        Product product = TestData.CreateProduct(category: "Dairy");
        ProductBatch batch = TestData.CreateBatch(product, 70);

        department.AddBatch(batch);

        Assert.Equal(70, department.CurrentQuantity);
        Assert.Equal(180, department.FreeCapacity);
        Assert.Equal(new[] { 50, 20, 0, 0, 0 },
            department.Shelves.Select(x => x.currentQuantity));
        Assert.Equal(0, batch.quantity);
        Assert.Equal(70, department.GetQuantity(product));
    }

    [Fact]
    public void Department_AddBatchRejectsWrongCategory()
    {
        Department department = new("Dairy");
        ProductBatch batch = TestData.CreateBatch(
            TestData.CreateProduct(category: "Sweets"));

        Assert.Throws<InvalidOperationException>(() => department.AddBatch(batch));
    }

    [Fact]
    public void Department_AddBatchRejectsNull()
    {
        Department department = new("Dairy");
        Assert.Throws<ArgumentNullException>(() => department.AddBatch(null!));
    }

    [Fact]
    public void Department_AddBatchRejectsInsufficientCapacity()
    {
        Department department = new("Dairy");
        Product product = TestData.CreateProduct();

        Assert.Throws<InvalidOperationException>(() =>
            department.AddBatch(TestData.CreateBatch(product, 251)));
    }

    [Fact]
    public void Department_AvailableQuantityAndRemoveExpiredWorkAcrossShelves()
    {
        Department department = new("Dairy");
        Product product = TestData.CreateProduct();
        department.AddBatch(TestData.CreateBatch(product, 3, -1));
        department.AddBatch(TestData.CreateBatch(product, 4, 0));
        department.AddBatch(TestData.CreateBatch(product, 5, 2));

        Assert.Equal(12, department.GetQuantity(product));
        Assert.Equal(9, department.GetAvailableQuantity(product, TestData.Today));
        Assert.Equal(3, department.RemoveExpiredBatches(TestData.Today));
        Assert.Equal(9, department.CurrentQuantity);
        Assert.Equal(9, department.GetAvailableQuantity(product, TestData.Today));
    }

    [Fact]
    public void Department_TakeProductUsesAvailableStockAcrossShelves()
    {
        Department department = new("Dairy");
        Product product = TestData.CreateProduct();
        department.AddBatch(TestData.CreateBatch(product, 50, 10));
        department.AddBatch(TestData.CreateBatch(product, 20, 2));

        var taken = department.TakeProduct(product, 55, TestData.Today);

        Assert.Equal(55, taken.Sum(x => x.quantity));
        Assert.Equal(15, department.GetQuantity(product));
    }

    [Fact]
    public void Department_TakeProductRejectsInvalidAndInsufficientQuantities()
    {
        Department department = new("Dairy");
        Product product = TestData.CreateProduct();

        Assert.Throws<ArgumentException>(() =>
            department.TakeProduct(product, 0, TestData.Today));
        Assert.Throws<InsufficientStockException>(() =>
            department.TakeProduct(product, 1, TestData.Today));
    }

    [Fact]
    public void Department_ContainsNoSeparateProductWhenQuantityIsZero()
    {
        Department department = new("Dairy");
        Assert.Equal(0, department.GetQuantity(TestData.CreateProduct()));
    }
}
