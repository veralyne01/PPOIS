namespace Minimarket
{
    public class InventoryCheck
    {
        private static int nextId = 0;
        private readonly List<InventoryCheckItem> items = new();

        public int id { get; }
        public DateOnly date { get; }
        public Employee responsibleEmployee { get; }
        public bool status { get; private set; }
        public IReadOnlyList<InventoryCheckItem> Items => items;

        public InventoryCheck(DateOnly date, Employee responsibleEmployee)
        {
            id = ++nextId;
            this.date = date;
            this.responsibleEmployee = responsibleEmployee
                ?? throw new ArgumentNullException(nameof(responsibleEmployee));
        }

        public void AddItem(InventoryCheckItem item)
        {
            if (status)
                throw new InvalidOperationException("The inventory check is already completed!");
            items.Add(item ?? throw new ArgumentNullException(nameof(item)));
        }

        public void Complete()
        {
            if (status)
                throw new InvalidOperationException("The inventory check is already completed!");
            status = true;
        }
    }
}
