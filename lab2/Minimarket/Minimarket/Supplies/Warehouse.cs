namespace Minimarket
{
    public class Warehouse
    {
        private readonly int capacity;
        private readonly List<ProductBatch> batches = new();

        public int CurrentQuantity =>
            batches.Sum(batch => batch.quantity);

        public int FreeCapacity =>
            capacity - CurrentQuantity;

        public IReadOnlyList<ProductBatch> Batches => batches;

        public Warehouse(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException(
                    "Capacity must be positive!");

            this.capacity = capacity;
        }

        public void AddBatch(ProductBatch batch)
        {
            if (batch == null)
                throw new ArgumentNullException(nameof(batch));

            if (batch.quantity > FreeCapacity)
                throw new InvalidOperationException(
                    "Not enough warehouse capacity!");

            batches.Add(batch);
        }

        public void ReceiveSupply(Supply supply)
        {
            if (supply == null)
                throw new ArgumentNullException(nameof(supply));

            int totalQuantity = supply.Batches.Sum(
                batch => batch.quantity
            );

            if (totalQuantity > FreeCapacity)
                throw new InvalidOperationException(
                    "Not enough warehouse capacity!");

            foreach (ProductBatch batch in supply.Batches)
            {
                ProductBatch warehouseBatch = batch.Copy();
                batches.Add(warehouseBatch);
            }


        }

        public int GetAvailableQuantity(
            Product product,
            DateOnly currentDate)
        {
            return batches
                .Where(batch =>
                    batch.product == product &&
                    batch.expirationDate >= currentDate)
                .Sum(batch => batch.quantity);
        }


        public int RemoveExpiredBatches(DateOnly currentDate)
        {
            List<ProductBatch> expired = batches
                .Where(batch => batch.expirationDate < currentDate)
                .ToList();

            int removedQuantity = expired.Sum(batch => batch.quantity);
            batches.RemoveAll(batch => batch.expirationDate < currentDate);

            return removedQuantity;
        }

        public List<ProductBatch> TakeForTransfer(
    Product product,
    int quantity,
    DateOnly currentDate)
        {
            if (quantity <= 0)
                throw new ArgumentException("Invalid quantity!");

            var availableBatches = batches
                .Where(batch =>
                    batch.product == product &&
                    batch.expirationDate >= currentDate)
                .OrderBy(batch => batch.expirationDate)
                .ToList();

            int availableQuantity =
                availableBatches.Sum(batch => batch.quantity);

            if (availableQuantity < quantity)
                throw new InsufficientStockException();

            List<ProductBatch> result = new();

            int remaining = quantity;

            foreach (ProductBatch batch in availableBatches)
            {
                int amount = Math.Min(
                    remaining,
                    batch.quantity
                );

                result.Add(batch.Take(amount));

                remaining -= amount;

                if (remaining == 0)
                    break;
            }

            batches.RemoveAll(batch => batch.quantity == 0);

            return result;
        }
    }
}
