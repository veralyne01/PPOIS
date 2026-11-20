namespace Minimarket
{
    public class StockTransfer
    {
        private readonly Warehouse warehouse;
        private readonly Department department;
        private readonly Product product;
        private readonly List<ProductBatch> batches;
        private readonly DateOnly date;
        private readonly List<StockMovement> movements = new();

        public int RequestedQuantity { get; }
        public int PlacedQuantity { get; private set; }
        public int ReturnedQuantity { get; private set; }
        public IReadOnlyList<StockMovement> Movements => movements;
        public bool IsCompleted { get; private set; }

        public StockTransfer(
            Warehouse warehouse,
            Department department,
            Product product,
            IEnumerable<ProductBatch> batches,
            int requestedQuantity,
            DateOnly date)
        {
            this.warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
            this.department = department ?? throw new ArgumentNullException(nameof(department));
            this.product = product ?? throw new ArgumentNullException(nameof(product));
            this.batches = batches?.ToList() ?? throw new ArgumentNullException(nameof(batches));

            if (requestedQuantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(requestedQuantity));
            if (this.batches.Any(batch => batch == null || batch.product != product))
                throw new InvalidOperationException("A transfer contains another product!");
            if (!string.Equals(product.category.CategoryName, department.name, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("The product does not belong to the target department!");
            if (this.batches.Sum(batch => batch.quantity) != requestedQuantity)
                throw new InvalidOperationException("Transferred quantity does not match the request!");

            RequestedQuantity = requestedQuantity;
            this.date = date;
        }

        public int Execute()
        {
            if (IsCompleted)
                throw new InvalidOperationException("The transfer has already been completed!");

            foreach (ProductBatch batch in batches)
            {
                if (batch.expirationDate < date)
                    throw new ExpiredProductException(
                        $"Batch {batch.Id} of {product.Name} has expired!");
            }

            foreach (ProductBatch batch in batches)
            {
                int amountToPlace = Math.Min(batch.quantity, department.FreeCapacity);

                if (amountToPlace > 0)
                {
                    ProductBatch part = batch.Take(amountToPlace);
                    department.AddBatch(part);
                    PlacedQuantity += amountToPlace;

                    movements.Add(new StockMovement(
                        product,
                        amountToPlace,
                        SupplyOperations.Transfer,
                        date,
                        Location.Warehouse,
                        new Location(department)
                    ));
                }

                if (batch.quantity > 0)
                {
                    warehouse.AddBatch(batch);
                    ReturnedQuantity += batch.quantity;
                }
            }

            IsCompleted = true;
            return PlacedQuantity;
        }
    }
}
