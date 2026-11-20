namespace Minimarket
{
    public class Product : Item
    {
        public int price { get; private set; }
        public string barcode { get; }
        public int shelfLifeDays { get; }

        public Product(
            string name,
            ProductCategory category,
            Manufacturer manufacturer,
            int price,
            string barcode,
            int shelfLifeDays)
            : base(name, category, manufacturer)
        {
            if (price <= 0 || shelfLifeDays <= 0)
                throw new InvalidProductDataException();

            this.price = price;
            this.barcode = barcode;
            this.shelfLifeDays = shelfLifeDays;
        }

        public void ChangePrice(int price)
        {
            if (price <= 0)
                throw new InvalidProductDataException();

            this.price = price;
        }
    }
}
