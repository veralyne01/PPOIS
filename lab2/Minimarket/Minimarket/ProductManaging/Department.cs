using System;
using System.Collections.Generic;
using System.Linq;

namespace Minimarket
{
    public class Department
    {
        private static int nextDepId = 100;

        public int Id { get; }
        public string name { get; }

        private readonly List<Shelf> shelves = new();

        public IReadOnlyList<Shelf> Shelves => shelves;

        public int FreeCapacity =>
            shelves.Sum(shelf => shelf.FreeCapacity);

        public int CurrentQuantity =>
            shelves.Sum(shelf => shelf.currentQuantity);

        public Department(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Department name cannot be empty!");

            Id = ++nextDepId;
            this.name = name;

            for (int i = 1; i <= 5; i++)
            {
                shelves.Add(new Shelf(i));
            }
        }

        public void AddBatch(ProductBatch batch)
        {
            if (batch == null)
                throw new ArgumentNullException(nameof(batch));

            if (batch.product.category.CategoryName != name)
                throw new InvalidOperationException(
                    "Product does not belong to this department!");

            if (batch.quantity > FreeCapacity)
                throw new InvalidOperationException(
                    "Not enough department capacity!");
            foreach (Shelf shelf in shelves)
            {
                if (batch.quantity == 0)
                    break;

                int amount = Math.Min(
                    batch.quantity,
                    shelf.FreeCapacity
                );

                if (amount == 0)
                    continue;

                ProductBatch part = batch.Take(amount);

                shelf.AddBatch(part);
            }
        }

        public int GetQuantity(Product product)
        {
            return shelves
                .SelectMany(shelf => shelf.Batches)
                .Where(batch => batch.product == product)
                .Sum(batch => batch.quantity);
        }

        public int RemoveExpiredBatches(DateOnly currentDate)
        {
            int removedQuantity = 0;

            foreach (Shelf shelf in shelves)
            {
                removedQuantity += shelf.RemoveExpiredBatches(currentDate);
            }

            return removedQuantity;
        }

        public int GetAvailableQuantity(Product product, DateOnly currentDate)
        {
            return shelves.Sum(shelf => shelf.GetAvailableQuantity(product, currentDate));
        }

        public List<ProductBatch> TakeProduct(
            Product product,
            int quantity,
            DateOnly currentDate)
        {
            if (quantity <= 0)
                throw new ArgumentException("Invalid quantity!");
            if (GetAvailableQuantity(product, currentDate) < quantity)
                throw new InsufficientStockException();

            List<ProductBatch> result = new();
            int remaining = quantity;
            foreach (Shelf shelf in shelves)
            {
                if (remaining == 0)
                    break;
                int amount = Math.Min(
                    remaining,
                    shelf.GetAvailableQuantity(product, currentDate)
                );
                if (amount == 0)
                    continue;
                result.AddRange(shelf.TakeProduct(product, amount, currentDate));
                remaining -= amount;
            }
            return result;
        }
    }
}
