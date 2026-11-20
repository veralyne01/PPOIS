namespace Minimarket
{
    public class Discount
    {
        public string Name { get; }
        public Product Product { get; }
        public int Percent { get; }
        public DateOnly StartDate { get; }
        public DateOnly EndDate { get; }

        public Discount(
            string name,
            Product product,
            int percent,
            DateOnly startDate,
            DateOnly endDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Discount name cannot be empty!");
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (percent <= 0 || percent >= 100)
                throw new ArgumentOutOfRangeException(nameof(percent));
            if (endDate < startDate)
                throw new ArgumentException("Invalid discount period!");

            Name = name;
            Product = product;
            Percent = percent;
            StartDate = startDate;
            EndDate = endDate;
        }

        public bool IsActive(DateOnly date)
        {
            return date >= StartDate && date <= EndDate;
        }

        public bool AppliesTo(Product product, DateOnly date)
        {
            return Product == product && IsActive(date);
        }

        public int CalculateDiscountPerUnit(DateOnly date)
        {
            if (!IsActive(date))
                return 0;

            return Product.price * Percent / 100;
        }
    }
}
