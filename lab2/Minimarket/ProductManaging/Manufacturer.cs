namespace Minimarket
{
    public class Manufacturer
    {
        public string ManufacturerName { get; }
        public Country country { get; }

        public Manufacturer(
            string name,
            Country country)
        {
            ManufacturerName = name;
            this.country = country;
        }
    }
}
