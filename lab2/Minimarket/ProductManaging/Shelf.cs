using System.Linq;

namespace Minimarket
{
    public class Shelf
    {
        private static int nextShelfId = 200;

        private readonly int id;
        private readonly List<ProductBatch> batches = new();

        public int number { get; }
        public int capacity { get; } = 50;

        public int currentQuantity =>
            batches.Sum(batch => batch.quantity);

        public int FreeCapacity =>
            capacity - currentQuantity;

        public IReadOnlyList<ProductBatch> Batches => batches;

        public Shelf(int number)
        {
            id = ++nextShelfId;
            this.number = number;
        }

        public void AddBatch(ProductBatch batch)
        {
            if (batch.quantity > FreeCapacity)
                throw new InvalidOperationException(
                    "Not enough shelf capacity!");

            batches.Add(batch);
        }

        public int GetAvailableQuantity(Product product, DateOnly currentDate)
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

        public List<ProductBatch> TakeProduct(
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

            if (availableBatches.Sum(batch => batch.quantity) < quantity)
                throw new InsufficientStockException();

            List<ProductBatch> result = new();
            int remaining = quantity;
            foreach (ProductBatch batch in availableBatches)
            {
                int amount = Math.Min(remaining, batch.quantity);
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
