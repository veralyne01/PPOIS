namespace Minimarket
{
    public class PurchaseItem
    {
        public Product product { get; }
        public int number { get; }
        public int price { get; }
        public int discount { get; }
        public int totalSum => (price - discount) * number;

        public PurchaseItem(Product product, int number, int discount = 0)
        {
            this.product = product ?? throw new ArgumentNullException(nameof(product));
            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            price = product.price;
            if (discount < 0 || discount > price)
                throw new ArgumentOutOfRangeException(nameof(discount));

            this.number = number;
            this.discount = discount;
        }
    }
}
