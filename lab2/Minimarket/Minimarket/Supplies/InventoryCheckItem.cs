namespace Minimarket
{
    public class InventoryCheckItem
    {
        public Product Product { get; }
        public int expectedAmount { get; }
        public int actualAmount { get; }
        public Location location { get; }
        public DateOnly expirationDate { get; }
        public int Difference => actualAmount - expectedAmount;
        public bool IsExpired(DateOnly date) => expirationDate < date;

        public InventoryCheckItem(
            Product product,
            int expectedAmount,
            int actualAmount,
            Location location,
            DateOnly expirationDate)
        {
            Product = product ?? throw new ArgumentNullException(nameof(product));
            if (expectedAmount < 0 || actualAmount < 0)
                throw new ArgumentOutOfRangeException(nameof(expectedAmount));

            this.expectedAmount = expectedAmount;
            this.actualAmount = actualAmount;
            this.location = location ?? throw new ArgumentNullException(nameof(location));
            this.expirationDate = expirationDate;
        }
    }
}
