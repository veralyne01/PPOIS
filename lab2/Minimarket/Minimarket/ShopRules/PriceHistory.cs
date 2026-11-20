namespace Minimarket
{
    public class PriceHistory
    {
        private readonly List<int> prices = new();

        public Product Product { get; }
        public IReadOnlyList<int> Prices => prices;
        public int CurrentPrice => prices.Count == 0 ? 0 : prices[^1];

        public PriceHistory(Product product, int initialPrice)
        {
            Product = product ?? throw new ArgumentNullException(nameof(product));
            if (initialPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(initialPrice));

            prices.Add(initialPrice);
        }

        public void AddPrice(int price)
        {
            if (price <= 0)
                throw new ArgumentOutOfRangeException(nameof(price));

            if (prices.Count == 0 || prices[^1] != price)
                prices.Add(price);
        }
    }
}
