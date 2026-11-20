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

        private bool isOpen = false;

        public IReadOnlyList<Department> Departments => departments;
        public IReadOnlyList<Employee> Employees => employees;
        public IReadOnlyList<CashRegister> CashRegisters => cashRegisters;
        public IReadOnlyList<Customer> Customers => customers;

        private readonly Warehouse warehouse = new(1000);
        private readonly ProductCatalog catalog = new();
        private readonly List<Supplier> suppliers = new();
        private int nextSupplyId = 1;

     
        private const int maxHistory = 5;
        private const int returnChance = 10; 
        private readonly List<Receipt> recentReceipts = new();
        private readonly List<Return> returns = new();
        public IReadOnlyList<Receipt> RecentReceipts => recentReceipts;
        public IReadOnlyList<Return> Returns => returns;
        private static readonly string[] returnReasons =
        {
            "Product is defective",
            "Changed my mind",
            "Bought by mistake",
            "Product did not fit"
        };

        public Warehouse Warehouse => warehouse;
        public ProductCatalog Catalog => catalog;

        public bool IsOpen => isOpen;

        public Shop()
        {
            CreateDepartments();
            CreateCashiers();
            CreateSpecialists();
            CreateProducts();
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
                Cashier cashier = new Cashier(
                    $"Cashier {i + 1}"
                );

                CashRegister register = new CashRegister();

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

            Manufacturer manufacturer =
                new("Belarusian Manufacturer", Country.Belarus);

            catalog.AddProduct(new Product(
                "Milk 2.5%",
                dairy,
                manufacturer,
                350,
                "481000000001",
                10
            ));

            catalog.AddProduct(new Product(
                "Chicken fillet",
                meat,
                manufacturer,
                1250,
                "481000000002",
                7
            ));

            catalog.AddProduct(new Product(
                "White bread",
                bread,
                manufacturer,
                250,
                "481000000003",
                3
            ));

            catalog.AddProduct(new Product(
                "Apples",
                fruit,
                manufacturer,
                420,
                "481000000004",
                20
            ));

            catalog.AddProduct(new Product(
                "Chocolate",
                sweets,
                manufacturer,
                550,
                "481000000005",
                180
            ));
        }
        private void FillWarehouse()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            // Создаём поставщика
            Supplier supplier = new Supplier("Initial Supplier");

            suppliers.Add(supplier);

            // Создаём первоначальную поставку
            Supply supply = new Supply(
                nextSupplyId++,
                supplier,
                today
            );

            // Формируем партии для каждого товара
            foreach (Product product in catalog.Products)
            {
                int quantity = Random.Shared.Next(20, 41);

                int daysToExpiration = Random.Shared.Next(
                    1,
                    product.shelfLifeDays + 1
                );

                DateOnly expirationDate =
                    today.AddDays(daysToExpiration);

                ProductBatch batch = new ProductBatch(
                    product,
                    quantity,
                    expirationDate
                );

                supply.AddBatch(batch);
            }

            // Склад принимает поставку, после чего поставщик сохраняет её в истории.
            warehouse.ReceiveSupply(supply);
            supplier.AddSupply(supply);
        }
        private void FillStoreFloor()
        {
            foreach (Product product in catalog.Products)
            {
                int desired = Random.Shared.Next(10, 21);
                RequestProductsFromWarehouse(product, desired);
            }
        }

        // Показывает товары в каждом отделе, группируя одинаковый товар
        // с одинаковым сроком годности. Указывает полки и доступное количество.
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
                    .GroupBy(x => new
                    {
                        x.Batch.product,
                        x.Batch.expirationDate
                    })
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

        // Удаляет просроченные товары из торгового зала и со склада.
        // Возвращает общее количество списанных единиц товара.
        public int RemoveExpiredProducts()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int removedQuantity = warehouse.RemoveExpiredBatches(today);

            foreach (Department department in departments)
            {
                removedQuantity += department.RemoveExpiredBatches(today);
            }

            Console.WriteLine(
                $"Expired products removed: {removedQuantity} unit(s).");

            return removedQuantity;
        }

        // Запрашивает указанное количество товара со склада и размещает его в отделе.
        // Если склада не хватает, недостающее количество автоматически поступает
        // новой поставкой. Если в отделе не хватает места, остаток возвращается на склад.
        public int RequestProductsFromWarehouse(Product product, int requestedQuantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (requestedQuantity <= 0)
                throw new ArgumentException("Quantity must be positive!");

            if (!catalog.Products.Contains(product))
                throw new ProductNotFoundException();

            Department? department = FindDepartment(product);
            if (department == null)
                throw new ProductNotFoundException(
                    "There is no department for this product!");

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            // Просроченный товар не должен занимать складскую вместимость.
            warehouse.RemoveExpiredBatches(today);

            int available = warehouse.GetAvailableQuantity(product, today);
            int missingQuantity = requestedQuantity - available;

            if (missingQuantity > 0)
            {
                ReplenishWarehouse(product, missingQuantity, today);
            }

            // Забираем запрошенное количество со склада по FEFO.
            List<ProductBatch> requestedBatches =
                warehouse.TakeForTransfer(product, requestedQuantity, today);

            int placedQuantity = 0;
            int returnedQuantity = 0;

            foreach (ProductBatch batch in requestedBatches)
            {
                int amountToPlace = Math.Min(
                    batch.quantity,
                    department.FreeCapacity
                );

                if (amountToPlace > 0)
                {
                    // Если места хватает не на всю партию, делим её.
                    ProductBatch part = batch.Take(amountToPlace);
                    department.AddBatch(part);
                    placedQuantity += amountToPlace;
                }

                // Всё, что не поместилось в отделе, возвращаем на склад.
                if (batch.quantity > 0)
                {
                    warehouse.AddBatch(batch);
                    returnedQuantity += batch.quantity;
                }
            }

            Console.WriteLine(
                $"Requested: {requestedQuantity} x {product.Name}. " +
                $"Placed in store: {placedQuantity}. " +
                $"Returned to warehouse: {returnedQuantity}.");

            return placedQuantity;
        }

        // Автоматически пополняет склад недостающим количеством товара новой поставкой.
        private void ReplenishWarehouse(
            Product product,
            int quantity,
            DateOnly currentDate)
        {
            if (quantity > warehouse.FreeCapacity)
                throw new InvalidOperationException(
                    "Not enough free capacity in warehouse for automatic replenishment!");

            Supplier? supplier = suppliers.FirstOrDefault();

            if (supplier == null)
            {
                supplier = new Supplier("Automatic Supplier");
                suppliers.Add(supplier);
            }

            Supply supply = new Supply(
                nextSupplyId++,
                supplier,
                currentDate
            );

            DateOnly expirationDate =
                currentDate.AddDays(product.shelfLifeDays);

            supply.AddBatch(new ProductBatch(
                product,
                quantity,
                expirationDate
            ));

            // Сначала склад принимает поставку. Только после успеха
            // поставка добавляется в историю поставщика.
            warehouse.ReceiveSupply(supply);
            supplier.AddSupply(supply);

            Console.WriteLine(
                $"Warehouse automatically replenished: " +
                $"{quantity} x {product.Name}.");
        }

        // Один шаг симуляции: покупатели ходят по залу, кассы обслуживают очередь,
        // возможно, кто-то приходит с возвратом
        public void NextStep()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            // 1. Покупатели ищут товары
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

            // 2. Кассы обслуживают покупателей
            foreach (CashRegister register in cashRegisters)
            {
                Customer? customer = register.NextStep(out Receipt? receipt);
                if (customer == null)
                    continue;

                customers.Remove(customer);
                if (receipt != null)
                {
                    AddToHistory(receipt);
                }
                else
                {
                    // Расплатиться не смог - товары возвращаются на полки
                    ReturnProductsToShelves(customer);
                }
            }

            // 3. Кто-то из последних покупателей может прийти с возвратом
            if (recentReceipts.Count > 0 && Random.Shared.Next(100) < returnChance)
            {
                MakeReturn();
            }
        }

        private Department? FindDepartment(Product product)
        {
            return departments.FirstOrDefault(
                department => department.name == product.category.CategoryName);
        }

        // Покупатель берёт с полки следующий товар из списка желаемого
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
            {
                customer.cart.AddBatch(batch);
            }
            Console.WriteLine($"{customer.name} took {product.Name} x{quantity}");
        }

        // Покупатель идёт на кассу с наименьшей очередью
        private void GoToCheckout(Customer customer)
        {
            if (customer.cart.IsEmpty)
            {
                Console.WriteLine($"{customer.name} left the shop with nothing");
                customers.Remove(customer);
                return;
            }

            CashRegister? register = cashRegisters
                .Where(r => r.IsOpen)
                .OrderBy(r => r.QueueLength)
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
                        // На полке не осталось места - товар списывается
                    }
                }
            }
            customer.cart.Clear();
        }

        // В истории хранятся только последние 5 покупателей
        private void AddToHistory(Receipt receipt)
        {
            recentReceipts.Add(receipt);
            while (recentReceipts.Count > maxHistory)
            {
                recentReceipts.RemoveAt(0);
            }
        }

        // Один из последних покупателей приходит к менеджеру по возвратам
        private void MakeReturn()
        {
            var candidates = recentReceipts
                .Where(r => r.Items.Any(i => r.GetReturnableQuantity(i.product) > 0))
                .ToList();
            if (candidates.Count == 0)
                return;

            Receipt receipt = candidates[Random.Shared.Next(candidates.Count)];
            var items = receipt.Items
                .Where(i => receipt.GetReturnableQuantity(i.product) > 0)
                .ToList();
            PurchaseItem item = items[Random.Shared.Next(items.Count)];
            int quantity = Random.Shared.Next(1, receipt.GetReturnableQuantity(item.product) + 1);
            string reason = returnReasons[Random.Shared.Next(returnReasons.Length)];

            ReturnManager manager = employees.OfType<ReturnManager>().First();
            Customer customer = receipt.purchase.customer;
            Console.WriteLine(
                $"{customer.name} came back with receipt #{receipt.id} " +
                $"to return {item.product.Name} x{quantity} ({reason})");
            try
            {
                Return result = manager.ProcessReturn(
                    receipt,
                    item.product,
                    quantity,
                    reason,
                    cashRegisters
                );
                returns.Add(result);
                Console.WriteLine(
                    $"{manager.Name} refunded {result.sum} ({receipt.payment.method})");
            }
            catch (Exception ex) when (
                ex is InvalidReturnException ||
                ex is ProductNotFoundException ||
                ex is InvalidChangeException)
            {
                Console.WriteLine($"Return refused: {ex.Message}");
            }
        }
    }
}
