namespace Minimarket
{
    public class ProductCatalog
    {
        private readonly List<Product> products = new();

        public IReadOnlyList<Product> Products => products;

        public void AddProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            products.Add(product);
        }
    }
}
