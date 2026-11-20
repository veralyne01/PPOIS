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
            Name = name;
            this.category = category;
            this.manufacturer = manufacturer;
        }
    }
}