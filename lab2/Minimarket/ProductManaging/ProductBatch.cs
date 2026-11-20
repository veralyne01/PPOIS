namespace Minimarket
{
    public class ProductBatch
    {
        public Product product { get; }
        public int quantity { get; private set; }
        public DateOnly expirationDate { get; }

        public ProductBatch(
            Product product,
            int quantity,
            DateOnly expirationDate)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantity <= 0)
                throw new ArgumentException(
                    "Quantity must be positive!");

            this.product = product;
            quantity = quantity;
            expirationDate = expirationDate;
        }

        public void RemoveQuantity(int quantity)
        {
            if (quantity <= 0 || quantity > this.quantity)
                throw new ArgumentException(
                    "Invalid quantity!");

            this.quantity -= quantity;
        }
    }
}