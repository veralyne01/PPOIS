namespace Minimarket
{
    public class ProductCategory
    {
        public string CategoryName { get; }

        public ProductCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Category name cannot be empty!", nameof(categoryName));

            CategoryName = categoryName;
        }
    }
}
