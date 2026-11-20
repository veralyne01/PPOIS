namespace Minimarket
{
    public class StockMovement
    {
        public Product Product { get; }
        public int Quantity { get; }
        public DateOnly Date { get; }
        public SupplyOperations Type { get; }
        public Location From { get; }
        public Location To { get; }

        public StockMovement(
            Product product,
            int quantity,
            SupplyOperations type,
            DateOnly date,
            Location from,
            Location to)
        {
            Product = product ?? throw new ArgumentNullException(nameof(product));
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            Quantity = quantity;
            Type = type;
            Date = date;
            From = from ?? throw new ArgumentNullException(nameof(from));
            To = to ?? throw new ArgumentNullException(nameof(to));
        }
    }
}
