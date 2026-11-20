namespace Minimarket
{
    public class InventoryReport
    {
        public InventoryCheck Check { get; }
        public DateOnly Date => Check.date;
        public IReadOnlyList<InventoryCheckItem> Items => Check.Items;

        public int TotalExpected => Items.Sum(item => item.expectedAmount);
        public int TotalActual => Items.Sum(item => item.actualAmount);
        public int Difference => TotalActual - TotalExpected;
        public int ExpiredQuantity => Items
            .Where(item => item.expirationDate < Date)
            .Sum(item => item.actualAmount);
        public int DiscrepancyCount => Items.Count(item => item.Difference != 0);
        public bool IsBalanced => DiscrepancyCount == 0;

        public InventoryReport(InventoryCheck check)
        {
            Check = check ?? throw new ArgumentNullException(nameof(check));
            if (!check.status)
                throw new InvalidOperationException("The inventory check is not completed!");
        }
    }
}
