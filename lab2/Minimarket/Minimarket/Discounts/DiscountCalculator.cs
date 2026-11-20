namespace Minimarket
{
    public class DiscountCalculator
    {
        private readonly List<Discount> discounts = new();

        public IReadOnlyList<Discount> Discounts => discounts;

        public void AddDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            discounts.Add(discount);
        }

        public int CalculateDiscountPerUnit(Product product, DateOnly date)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return discounts
                .Where(discount => discount.AppliesTo(product, date))
                .Select(discount => discount.CalculateDiscountPerUnit(date))
                .DefaultIfEmpty(0)
                .Max();
        }

        public int CalculateTotalDiscount(ShoppingCart cart, DateOnly date)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            return cart.ProductList.Sum(item =>
                CalculateDiscountPerUnit(item.product, date) * item.number);
        }
    }
}
