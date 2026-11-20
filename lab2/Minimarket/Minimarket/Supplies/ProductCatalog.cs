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

            if (products.Any(existing =>
                string.Equals(existing.barcode, product.barcode, StringComparison.Ordinal)))
                throw new InvalidProductDataException("A product with this barcode is already registered!");

            products.Add(product);
        }

        public Product? FindByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                throw new ArgumentException("Barcode cannot be empty!", nameof(barcode));

            return products.FirstOrDefault(product =>
                string.Equals(product.barcode, barcode, StringComparison.Ordinal));
        }

        public Product? FindByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty!", nameof(name));

            return products.FirstOrDefault(product =>
                string.Equals(product.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
