using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}