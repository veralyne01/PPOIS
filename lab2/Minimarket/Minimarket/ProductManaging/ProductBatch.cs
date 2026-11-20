namespace Minimarket
{
    public class ProductBatch
    {
        private static int nextId = 0;

        public int Id { get; }
        public Product product { get; }
        public int quantity { get; private set; }
        public DateOnly expirationDate { get; }
        public Label label { get; }

        public ProductBatch(
            Product product,
            int quantity,
            DateOnly expirationDate)
            : this(++nextId, product, quantity, expirationDate)
        {
        }

        private ProductBatch(
            int id,
            Product product,
            int quantity,
            DateOnly expirationDate)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantity <= 0)
                throw new ArgumentException(
                    "Quantity must be positive!");

            Id = id;
            this.product = product;
            this.quantity = quantity;
            this.expirationDate = expirationDate;
            label = new Label(this);
        }

        public ProductBatch Take(int amount)
        {
            if (amount <= 0 || amount > quantity)
                throw new ArgumentException(
                    "Invalid quantity!");

            quantity -= amount;

            return new ProductBatch(
                Id,
                product,
                amount,
                expirationDate
            );
        }

        public void RemoveQuantity(int amount)
        {
            if (amount <= 0 || amount > quantity)
                throw new ArgumentException(
                    "Invalid quantity!");

            quantity -= amount;
        }
        public ProductBatch Copy()
        {
            return new ProductBatch(
                Id,
                product,
                quantity,
                expirationDate
            );
        }
    }
}
