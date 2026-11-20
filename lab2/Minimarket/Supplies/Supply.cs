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
            this.id = id;
            this.supplier = supplier;
            this.date = date;
        }

        public void AddBatch(ProductBatch batch)
        {
            if (batch == null)
                throw new ArgumentNullException(nameof(batch));

            batches.Add(batch);
        }
    }
}