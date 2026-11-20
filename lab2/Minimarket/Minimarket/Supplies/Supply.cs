namespace Minimarket
{
    public class Supply
    {
        public int id { get; }
        public Supplier supplier { get; }
        public DateOnly date { get; }

        private readonly List<ProductBatch> batches = new();

        public IReadOnlyList<ProductBatch> Batches => batches;

        public Supply(int id, Supplier supplier, DateOnly date)
        {
            if (id <= 0)
                throw new InvalidSupplyException("Supply ID must be positive!");

            this.id = id;
            this.supplier = supplier
                ?? throw new SupplierNotFoundException("A supply must belong to a supplier!");
            this.date = date;
        }

        public void AddBatch(ProductBatch batch)
        {
            if (batch == null)
                throw new ArgumentNullException(nameof(batch));
            if (batch.expirationDate < date)
                throw new InvalidSupplyException(
                    "A supply cannot contain a batch that expired before delivery!");

            batches.Add(batch);
        }
    }
}
