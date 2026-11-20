namespace Minimarket
{
    public class Return
    {
        private static int nextReturnId = 0;
        public int id { get; }
        public string? reason { get; }
        public int sum { get; }
        public DateOnly date { get; }
        public Receipt receipt { get; }
        public Dictionary<Product, int> products { get; }

        public Return(Receipt receipt, Product product, int quantity, int sum, string? reason)
        {
            id = ++nextReturnId;
            this.receipt = receipt;
            this.sum = sum;
            this.reason = reason;
            date = DateOnly.FromDateTime(DateTime.Today);
            products = new Dictionary<Product, int> { { product, quantity } };
        }
    }
}
