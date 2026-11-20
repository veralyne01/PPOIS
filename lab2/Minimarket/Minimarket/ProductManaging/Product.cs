namespace Minimarket
{
    public class Product : Item
    {
        public int price { get; private set; }
        public string barcode { get; }
        public int shelfLifeDays { get; }
        public PriceHistory PriceHistory { get; }

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

            if (string.IsNullOrWhiteSpace(barcode) || barcode.Any(character => !char.IsDigit(character)))
                throw new InvalidBarcodeException();

            this.price = price;
            this.barcode = barcode;
            this.shelfLifeDays = shelfLifeDays;
            PriceHistory = new PriceHistory(this, price);
        }

        public void ChangePrice(int price)
        {
            if (price <= 0)
                throw new InvalidProductDataException();

            PriceHistory.AddPrice(price);
            this.price = price;
        }
    }
}
