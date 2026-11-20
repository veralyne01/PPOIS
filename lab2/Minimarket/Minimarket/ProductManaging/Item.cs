namespace Minimarket
{
    public class Item
    {
        public string Name { get; }
        public ProductCategory category { get; }
        public Manufacturer manufacturer { get; }

        protected Item(
            string name,
            ProductCategory category,
            Manufacturer manufacturer)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidProductDataException("Product name cannot be empty!");

            Name = name;
            this.category = category ?? throw new ArgumentNullException(nameof(category));
            this.manufacturer = manufacturer ?? throw new ArgumentNullException(nameof(manufacturer));
        }
    }
}
