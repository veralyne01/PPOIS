namespace Minimarket
{
    public class Supplier
    {
        public string name { get; }

        private readonly List<Supply> supplies = new();

        public IReadOnlyList<Supply> Supplies => supplies;

        public Supplier(string name)
        {
            this.name = name;
        }

        public void AddSupply(Supply supply)
        {
            if (supply.supplier != this)
                throw new ArgumentException(
                    "Supply belongs to another supplier!");

            supplies.Add(supply);
        }
    }
}
