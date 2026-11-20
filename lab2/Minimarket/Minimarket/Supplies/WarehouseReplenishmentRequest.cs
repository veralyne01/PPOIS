namespace Minimarket
{
    public class WarehouseReplenishmentRequest
    {
        public Supplier supplier { get; }
        public Product product { get; }
        public int requestedAmount { get; }
        public DateOnly date { get; }
        public bool isApproved { get; private set; }

        public WarehouseReplenishmentRequest(
            Supplier? supplier,
            Product product,
            int requestedAmount,
            DateOnly date)
        {
            this.supplier = supplier
                ?? throw new SupplierNotFoundException("A supplier is required for a replenishment request!");
            this.product = product
                ?? throw new ArgumentNullException(nameof(product));
            if (requestedAmount <= 0)
                throw new InvalidSupplyException("Requested supply quantity must be positive!");

            this.requestedAmount = requestedAmount;
            this.date = date;
        }

        public void Approve()
        {
            isApproved = true;
        }

        public void Reject()
        {
            isApproved = false;
        }
    }
}
