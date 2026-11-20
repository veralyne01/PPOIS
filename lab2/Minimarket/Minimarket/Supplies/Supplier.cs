namespace Minimarket
{
    public class Supplier
    {
        public string name { get; }

        private readonly List<Supply> supplies = new();

        public IReadOnlyList<Supply> Supplies => supplies;

        public Supplier(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Supplier name cannot be empty!", nameof(name));

            this.name = name;
        }

        public void AddSupply(Supply supply)
        {
            if (supply == null)
                throw new ArgumentNullException(nameof(supply));

            if (!ReferenceEquals(supply.supplier, this))
                throw new ArgumentException(
                    "Supply belongs to another supplier!");

            supplies.Add(supply);
        }
    }
}
