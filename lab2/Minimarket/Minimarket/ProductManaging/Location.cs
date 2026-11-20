namespace Minimarket
{
    public class Location
    {
        public string Name { get; }
        public Department? Department { get; }
        public Shelf? Shelf { get; }

        public Location(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Location name cannot be empty!");

            Name = name;
        }

        public Location(Department department)
        {
            Department = department ?? throw new ArgumentNullException(nameof(department));
            Name = department.name;
        }

        public Location(Department department, Shelf shelf)
        {
            Department = department ?? throw new ArgumentNullException(nameof(department));
            Shelf = shelf ?? throw new ArgumentNullException(nameof(shelf));

            if (!department.Shelves.Contains(shelf))
                throw new ArgumentException("The shelf does not belong to this department!");

            Name = $"{department.name}, shelf {shelf.number}";
        }

        public static Location Warehouse => new("Warehouse");

        public override string ToString() => Name;
    }
}
