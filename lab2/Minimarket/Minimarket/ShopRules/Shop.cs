using System;
using System.Collections.Generic;
using System.Linq;

namespace Minimarket
{
    public class Shop
    {
        private readonly List<Department> departments = new();
        private readonly List<Employee> employees = new();
        private readonly List<CashRegister> cashRegisters = new();
        private readonly List<Customer> customers = new();
        private readonly Warehouse warehouse = new(1000);
        private readonly ProductCatalog catalog = new();
        private readonly List<Supplier> suppliers = new();
        private readonly List<Receipt> recentReceipts = new();
        private readonly List<Return> returns = new();
        private readonly List<ShopReplenishmentOrder> replenishmentOrders = new();
        private readonly List<WarehouseReplenishmentRequest> warehouseRequests = new();
        private readonly List<StockMovement> stockMovements = new();
        private readonly List<InventoryCheck> inventoryChecks = new();
        private readonly List<ScheduledReturn> scheduledReturns = new();
        private readonly DiscountCalculator discountCalculator = new();

        private bool isOpen = false;
        private int nextSupplyId = 1;
        private int currentStep = 0;

        private const int maxHistory = 5;
        private const int returnChance = 10;

        private static readonly string[] returnReasons =
        {
            "Product is defective",
            "Changed my mind",
            "Bought by mistake",
            "Product did not fit"
        };

        public IReadOnlyList<Department> Departments => departments;
        public IReadOnlyList<Employee> Employees => employees;
        public IReadOnlyList<CashRegister> CashRegisters => cashRegisters;
        public IReadOnlyList<Customer> Customers => customers;
        public IReadOnlyList<Receipt> RecentReceipts => recentReceipts;
        public IReadOnlyList<Return> Returns => returns;
        public IReadOnlyList<ShopReplenishmentOrder> ReplenishmentOrders => replenishmentOrders;
        public IReadOnlyList<WarehouseReplenishmentRequest> WarehouseRequests => warehouseRequests;
        public IReadOnlyList<StockMovement> StockMovements => stockMovements;
        public IReadOnlyList<InventoryCheck> InventoryChecks => inventoryChecks;
        public IReadOnlyList<ScheduledReturn> ScheduledReturns => scheduledReturns;

        public Warehouse Warehouse => warehouse;
        public ProductCatalog Catalog => catalog;
        public DiscountCalculator DiscountCalculator => discountCalculator;
        public bool IsOpen => isOpen;

        public Shop()
        {
            CreateDepartments();
            CreateProducts();
            CreateDiscounts();
            CreateCashiers();
            CreateSpecialists();
            FillWarehouse();
            FillStoreFloor();
        }

        private void CreateDepartments()
        {
            string[] departmentNames =
            {
                "Dairy",
                "Meat and fish",
                "Bread and cereal",
                "Fruit and vegetables",
                "Sweets"
            };

            foreach (string name in departmentNames)
            {
                departments.Add(new Department(name));
            }
        }

        private void CreateCashiers()
        {
            for (int i = 0; i < 4; i++)
            {
                Cashier cashier = new Cashier($"Cashier {i + 1}");
                CashRegister register = new CashRegister(discountCalculator);
                register.AssignCashier(cashier);

                employees.Add(cashier);
                cashRegisters.Add(register);
            }
        }

        private void CreateSpecialists()
        {
            employees.Add(new InventoryManager("Inventory Manager"));
            employees.Add(new ReturnManager("Return Manager"));
        }

        public void AddCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));
            customers.Add(customer);
        }

        public void Open()
        {
            if (isOpen)
                throw new ShopAlreadyOpenedException();

            foreach (CashRegister register in cashRegisters)
            {
                register.Open();
            }

            isOpen = true;
            Console.WriteLine("Shop successfully opened!");
        }

        private void CreateProducts()
        {
            ProductCategory dairy = new("Dairy");
            ProductCategory meat = new("Meat and fish");
            ProductCategory bread = new("Bread and cereal");
            ProductCategory fruit = new("Fruit and vegetables");
            ProductCategory sweets = new("Sweets");

            Manufacturer manufacturer = new("Belarusian Manufacturer", Country.Belarus);

            catalog.AddProduct(new Product("Milk 2.5%", dairy, manufacturer, 350, "481000000001", 10));
            catalog.AddProduct(new Product("Chicken fillet", meat, manufacturer, 1250, "481000000002", 7));
            catalog.AddProduct(new Product("White bread", bread, manufacturer, 250, "481000000003", 3));
            catalog.AddProduct(new Product("Apples", fruit, manufacturer, 420, "481000000004", 20));
            catalog.AddProduct(new Product("Chocolate", sweets, manufacturer, 550, "481000000005", 180));
        }

        private void CreateDiscounts()
        {
            Product chocolate = catalog.Products.First(product => product.Name == "Chocolate");
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            discountCalculator.AddDiscount(new Discount(
                "Welcome chocolate discount",
                chocolate,
                10,
                today,
                today.AddDays(7)
            ));
        }

        private void FillWarehouse()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            Supplier supplier = new Supplier("Initial Supplier");
            suppliers.Add(supplier);

            Supply supply = new Supply(nextSupplyId++, supplier, today);

            foreach (Product product in catalog.Products)
            {
                int quantity = Random.Shared.Next(20, 41);
                int daysToExpiration = Random.Shared.Next(1, product.shelfLifeDays + 1);
                DateOnly expirationDate = today.AddDays(daysToExpiration);
                supply.AddBatch(new ProductBatch(product, quantity, expirationDate));
            }

            warehouse.ReceiveSupply(supply);
            supplier.AddSupply(supply);

            foreach (ProductBatch batch in supply.Batches)
            {
                stockMovements.Add(new StockMovement(
                    batch.product,
                    batch.quantity,
                    SupplyOperations.Arrival,
                    today,
                    new Location($"Supplier: {supplier.name}"),
                    Location.Warehouse
                ));
            }
        }

        private void FillStoreFloor()
        {
            foreach (Product product in catalog.Products)
            {
                int desired = Random.Shared.Next(10, 21);
                RequestProductsFromWarehouse(product, desired);
            }
        }

        public void DisplayStoreInventory()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            foreach (Department department in departments)
            {
                Console.WriteLine($"\n========== {department.name} ==========");

                var departmentBatches = department.Shelves
                    .SelectMany(shelf => shelf.Batches)
                    .ToList();

                int availableQuantity = departmentBatches
                    .Where(batch => batch.expirationDate >= today)
                    .Sum(batch => batch.quantity);

                Console.WriteLine(
                    $"Total: {department.CurrentQuantity}, " +
                    $"available: {availableQuantity}, " +
                    $"free capacity: {department.FreeCapacity}");

                var batches = department.Shelves
                    .SelectMany(shelf => shelf.Batches.Select(batch => new
                    {
                        Shelf = shelf,
                        Batch = batch
                    }))
                    .GroupBy(x => new { x.Batch.product, x.Batch.expirationDate })
                    .OrderBy(group => group.Key.product.Name)
                    .ThenBy(group => group.Key.expirationDate);

                bool hasProducts = false;
                foreach (var group in batches)
                {
                    hasProducts = true;
                    int quantity = group.Sum(x => x.Batch.quantity);
                    string shelfNumbers = string.Join(", ",
                        group.Select(x => x.Shelf.number).Distinct().OrderBy(x => x));
                    bool isExpired = group.Key.expirationDate < today;
                    string status = isExpired ? "EXPIRED" : "AVAILABLE";

                    Console.WriteLine(
                        $"{group.Key.product.Name} | " +
                        $"quantity: {quantity} | " +
                        $"expiration: {group.Key.expirationDate:dd.MM.yyyy} | " +
                        $"shelf(s): {shelfNumbers} | {status}");
                }

                if (!hasProducts)
                    Console.WriteLine("No products in this department.");
            }
        }

        public int RemoveExpiredProducts()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int removedQuantity = 0;

            foreach (ProductBatch batch in warehouse.Batches
                .Where(batch => batch.expirationDate < today).ToList())
            {
                stockMovements.Add(new StockMovement(
                    batch.product, batch.quantity, SupplyOperations.WriteOff,
                    today, Location.Warehouse, new Location("Write-off")
                ));
            }
            removedQuantity += warehouse.RemoveExpiredBatches(today);

            foreach (Department department in departments)
            {
                foreach (Shelf shelf in department.Shelves)
                {
                    foreach (ProductBatch batch in shelf.Batches
                        .Where(batch => batch.expirationDate < today).ToList())
                    {
                        stockMovements.Add(new StockMovement(
                            batch.product, batch.quantity, SupplyOperations.WriteOff,
                            today, new Location(department, shelf), new Location("Write-off")
                        ));
                    }
                }

                removedQuantity += department.RemoveExpiredBatches(today);
            }

            Console.WriteLine($"Expired products removed: {removedQuantity} unit(s).");
            return removedQuantity;
        }

        public int RequestProductsFromWarehouse(Product product, int requestedQuantity)
        {
            return RequestProductsFromWarehouse(product, requestedQuantity, null);
        }

        public int RequestProductsFromWarehouse(
            Product product,
            int requestedQuantity,
            string? supplierName)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (requestedQuantity <= 0)
                throw new ArgumentException("Quantity must be positive!");
            Product? registeredProduct = catalog.FindByBarcode(product.barcode);
            if (registeredProduct == null)
                throw new ProductNotFoundException();

            // Continue with the canonical instance from the catalog.
            product = registeredProduct;

            Department? department = FindDepartment(product);
            if (department == null)
                throw new ProductNotFoundException("There is no department for this product!");

            Supplier? selectedSupplier = null;
            if (!string.IsNullOrWhiteSpace(supplierName))
                selectedSupplier = FindSupplier(supplierName);

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            warehouse.RemoveExpiredBatches(today);

            int available = warehouse.GetAvailableQuantity(product, today);
            int missingQuantity = requestedQuantity - available;
            if (missingQuantity > 0)
            {
                ReplenishWarehouse(product, missingQuantity, today, selectedSupplier);
            }

            List<ProductBatch> requestedBatches =
                warehouse.TakeForTransfer(product, requestedQuantity, today);

            StockTransfer transfer = new(
                warehouse,
                department,
                product,
                requestedBatches,
                requestedQuantity,
                today
            );

            int placedQuantity = transfer.Execute();
            stockMovements.AddRange(transfer.Movements);

            ShopReplenishmentOrder order = new(product, requestedQuantity);
            order.Complete(transfer);
            replenishmentOrders.Add(order);

            Console.WriteLine(
                $"Requested: {requestedQuantity} x {product.Name}. " +
                $"Placed in store: {placedQuantity}. " +
                $"Returned to warehouse: {transfer.ReturnedQuantity}.");

            return placedQuantity;
        }

        private void ReplenishWarehouse(
            Product product,
            int quantity,
            DateOnly currentDate,
            Supplier? selectedSupplier = null)
        {
            if (quantity > warehouse.FreeCapacity)
                throw new InvalidOperationException(
                    "Not enough free capacity in warehouse for automatic replenishment!");

            Supplier? supplier = selectedSupplier ?? suppliers.FirstOrDefault();
            if (supplier == null)
            {
                supplier = new Supplier("Automatic Supplier");
                suppliers.Add(supplier);
            }

            WarehouseReplenishmentRequest request = new(
                supplier, product, quantity, currentDate);
            request.Approve();
            warehouseRequests.Add(request);

            Supply supply = new Supply(nextSupplyId++, supplier, currentDate);
            DateOnly expirationDate = currentDate.AddDays(product.shelfLifeDays);
            ProductBatch batch = new(product, quantity, expirationDate);
            supply.AddBatch(batch);

            warehouse.ReceiveSupply(supply);
            supplier.AddSupply(supply);

            stockMovements.Add(new StockMovement(
                product,
                quantity,
                SupplyOperations.Arrival,
                currentDate,
                new Location($"Supplier: {supplier.name}"),
                Location.Warehouse
            ));

            Console.WriteLine($"Warehouse automatically replenished: {quantity} x {product.Name}.");
        }

        public Supplier FindSupplier(string name)
        {
            Supplier? supplier = suppliers.FirstOrDefault(
                supplier => string.Equals(supplier.name, name, StringComparison.OrdinalIgnoreCase));

            return supplier ?? throw new SupplierNotFoundException(
                $"Supplier '{name}' was not found!");
        }

        public InventoryReport PerformInventoryCheck()
        {
            InventoryManager manager = employees.OfType<InventoryManager>().First();
            InventoryReport report = manager.PerformInventoryCheck(
                warehouse,
                departments,
                DateOnly.FromDateTime(DateTime.Today)
            );
            inventoryChecks.Add(report.Check);
            return report;
        }

        public Return ProcessReturn(
            int receiptId,
            Product product,
            int quantity,
            string? reason)
        {
            Receipt? receipt = recentReceipts.FirstOrDefault(receipt => receipt.id == receiptId);
            if (receipt == null)
                throw new ReceiptNotFoundException($"Receipt #{receiptId} was not found!");

            ReturnManager manager = employees.OfType<ReturnManager>().First();
            Return result = manager.ProcessReturn(receipt, product, quantity, reason, cashRegisters);
            returns.Add(result);
            return result;
        }

        public void NextStep()
        {
            currentStep++;
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            foreach (Customer customer in customers
                .Where(c => c.status == Status.LookingForProducts)
                .ToList())
            {
                customer.timer--;
                if (customer.timer > 0)
                    continue;

                if (customer.HasWishes)
                    PickProduct(customer, today);

                if (customer.HasWishes)
                    customer.timer = Random.Shared.Next(1, 4);
                else
                    GoToCheckout(customer);
            }

            foreach (CashRegister register in cashRegisters)
            {
                Customer? customer = register.NextStep(out Receipt? receipt);
                if (customer == null)
                    continue;

                customers.Remove(customer);
                if (receipt != null)
                    AddToHistory(receipt);
                else
                    ReturnProductsToShelves(customer);
            }

            ScheduleRandomReturn();
            ProcessScheduledReturns();
        }

        private Department? FindDepartment(Product product)
        {
            return departments.FirstOrDefault(
                department => department.name == product.category.CategoryName);
        }

        private void PickProduct(Customer customer, DateOnly today)
        {
            var wish = customer.TakeNextWish();
            Product product = wish.Key;
            Department? department = FindDepartment(product);

            if (department == null)
            {
                Console.WriteLine($"{customer.name}: no department for {product.Name}");
                return;
            }

            int available = department.GetAvailableQuantity(product, today);
            if (available == 0)
            {
                Console.WriteLine($"{customer.name} could not find {product.Name}");
                return;
            }

            int quantity = Math.Min(wish.Value, available);
            foreach (ProductBatch batch in department.TakeProduct(product, quantity, today))
                customer.cart.AddBatch(batch);

            Console.WriteLine($"{customer.name} took {product.Name} x{quantity}");
        }

        private void GoToCheckout(Customer customer)
        {
            if (customer.cart.IsEmpty)
            {
                Console.WriteLine($"{customer.name} left the shop with nothing");
                customers.Remove(customer);
                return;
            }

            CashRegister? register = cashRegisters
                .Where(register => register.IsOpen)
                .OrderBy(register => register.QueueLength)
                .FirstOrDefault();

            if (register == null)
            {
                Console.WriteLine($"{customer.name} left: all registers are closed");
                ReturnProductsToShelves(customer);
                customers.Remove(customer);
                return;
            }

            register.Enqueue(customer);
            Console.WriteLine(
                $"{customer.name} is in line at register {register.Id} " +
                $"(people ahead: {register.QueueLength - 1})");
        }

        private void ReturnProductsToShelves(Customer customer)
        {
            foreach (CartItem item in customer.cart.ProductList)
            {
                Department? department = FindDepartment(item.product);
                if (department == null)
                    continue;

                foreach (ProductBatch batch in item.Batches)
                {
                    try
                    {
                        department.AddBatch(batch);
                    }
                    catch (InvalidOperationException)
                    {
                        // На полке не осталось места - товар остаётся вне продажи.
                    }
                }
            }
            customer.cart.Clear();
        }

        private void AddToHistory(Receipt receipt)
        {
            recentReceipts.Add(receipt);
            while (recentReceipts.Count > maxHistory)
                recentReceipts.RemoveAt(0);
        }

        private void ScheduleRandomReturn()
        {
            if (recentReceipts.Count == 0 || Random.Shared.Next(100) >= returnChance)
                return;

            var candidates = recentReceipts
                .SelectMany(receipt => receipt.Items.Select(item =>
                {
                    int pending = scheduledReturns
                        .Where(scheduled => !scheduled.IsProcessed &&
                            scheduled.Receipt == receipt &&
                            scheduled.Product == item.product)
                        .Sum(scheduled => scheduled.Quantity);

                    int returnable = receipt.GetReturnableQuantity(item.product) - pending;
                    return new { Receipt = receipt, Item = item, Returnable = returnable };
                }))
                .Where(candidate => candidate.Returnable > 0)
                .ToList();

            if (candidates.Count == 0)
                return;

            var selected = candidates[Random.Shared.Next(candidates.Count)];
            int quantity = Random.Shared.Next(1, selected.Returnable + 1);
            string reason = returnReasons[Random.Shared.Next(returnReasons.Length)];

            scheduledReturns.Add(new ScheduledReturn(
                selected.Receipt,
                selected.Item.product,
                quantity,
                reason,
                currentStep + Random.Shared.Next(1, 6)
            ));
        }

        private void ProcessScheduledReturns()
        {
            foreach (ScheduledReturn scheduled in scheduledReturns
                .Where(item => item.IsDue(currentStep)).ToList())
            {
                try
                {
                    ReturnManager manager = employees.OfType<ReturnManager>().First();
                    Return result = manager.ProcessReturn(
                        scheduled.Receipt,
                        scheduled.Product,
                        scheduled.Quantity,
                        scheduled.Reason,
                        cashRegisters
                    );
                    returns.Add(result);
                    scheduled.MarkProcessed();
                    Console.WriteLine(
                        $"Return Manager processed return #{result.id} " +
                        $"for receipt #{scheduled.Receipt.id}.");
                }
                catch (Exception ex) when (
                    ex is InvalidReturnException ||
                    ex is ProductNotFoundException ||
                    ex is InvalidChangeException)
                {
                    Console.WriteLine($"Return refused: {ex.Message}");
                    scheduled.MarkProcessed();
                }
            }

            scheduledReturns.RemoveAll(item => item.IsProcessed);
        }

        // Оставлен для ручного вызова и существующих сценариев тестирования.
        private void MakeReturn()
        {
            var candidates = recentReceipts
                .Where(receipt => receipt.Items.Any(
                    item => receipt.GetReturnableQuantity(item.product) > 0))
                .ToList();
            if (candidates.Count == 0)
                return;

            Receipt receipt = candidates[Random.Shared.Next(candidates.Count)];
            var items = receipt.Items
                .Where(item => receipt.GetReturnableQuantity(item.product) > 0)
                .ToList();
            PurchaseItem item = items[Random.Shared.Next(items.Count)];
            int quantity = Random.Shared.Next(1, receipt.GetReturnableQuantity(item.product) + 1);
            string reason = returnReasons[Random.Shared.Next(returnReasons.Length)];

            ScheduledReturn scheduled = new(
                receipt, item.product, quantity, reason, currentStep);
            scheduledReturns.Add(scheduled);
            ProcessScheduledReturns();
        }
    }
}
