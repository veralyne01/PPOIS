namespace Minimarket
{
    public class Purchase
    {
        private static int nextPurchaseId = 0;
        public int id { get; }
        public Customer customer { get; }
        public Cashier cashier { get; }
        public DateOnly date { get; }
        private readonly List<PurchaseItem> items;
        public IReadOnlyList<PurchaseItem> Items => items;
        public Payment payment { get; }
        public int bonusesSpent { get; }
        public int bonusesAccrued { get; }
        public int total => items.Sum(item => item.totalSum);

        public Purchase(
            Customer customer,
            Cashier cashier,
            List<PurchaseItem> items,
            Payment payment,
            int bonusesSpent,
            int bonusesAccrued)
        {
            id = ++nextPurchaseId;
            this.customer = customer;
            this.cashier = cashier;
            this.items = items;
            this.payment = payment;
            this.bonusesSpent = bonusesSpent;
            this.bonusesAccrued = bonusesAccrued;
            date = DateOnly.FromDateTime(DateTime.Today);
        }
    }
}
