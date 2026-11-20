namespace Minimarket
{
    public class Manufacturer
    {
        public string ManufacturerName { get; }
        public Country country { get; }

        public Manufacturer(string name, Country country)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Manufacturer name cannot be empty!", nameof(name));

            ManufacturerName = name;
            this.country = country;
        }
    }
}
