namespace Minimarket
{
    public class ShopReplenishmentOrder
    {
        private static int nextId = 0;

        public int id { get; }
        public Product product { get; }
        public int requestedAmount { get; }
        public int placedAmount { get; private set; }
        public int returnedAmount { get; private set; }
        public bool isSatisfied { get; private set; }
        public bool isCompleted { get; private set; }

        public ShopReplenishmentOrder(Product product, int requestedAmount)
        {
            this.product = product ?? throw new ArgumentNullException(nameof(product));
            if (requestedAmount <= 0)
                throw new ArgumentOutOfRangeException(nameof(requestedAmount));

            id = ++nextId;
            this.requestedAmount = requestedAmount;
        }

        public void Complete(StockTransfer transfer)
        {
            if (transfer == null)
                throw new ArgumentNullException(nameof(transfer));
            if (!transfer.IsCompleted)
                throw new InvalidOperationException("The stock transfer is not completed!");
            if (transfer.RequestedQuantity != requestedAmount)
                throw new InvalidOperationException("The transfer does not match this order!");

            placedAmount = transfer.PlacedQuantity;
            returnedAmount = transfer.ReturnedQuantity;
            isSatisfied = placedAmount == requestedAmount;
            isCompleted = true;
        }
    }
}
