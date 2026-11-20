namespace Minimarket
{
    public class CartItem
    {
        public Product product { get; }
        private readonly List<ProductBatch> batches = new();
        public IReadOnlyList<ProductBatch> Batches => batches;

        public CartItem(Product product)
        {
            this.product = product;
        }

        public int number;
        public int totalSum => product.price * number;

        public void AddBatch(ProductBatch batch)
        {
            batches.Add(batch);
            number += batch.quantity;
        }

        public int IncreaseNumber() { return number++; }
        public int DecreaseNumber() { return number--; }
    }
}
