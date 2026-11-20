namespace Minimarket
{
    public class ShoppingCart
    {
        private int id;
        private static int nextCartId = 0;

        public ShoppingCart()
        {
            id = ++nextCartId;
        }

        private readonly List<CartItem> productList = new();
        public IReadOnlyList<CartItem> ProductList => productList;
        public bool IsEmpty => productList.Count == 0;
        public void AddBatch(ProductBatch batch)
        {
            CartItem? item = productList.FirstOrDefault(
                cartItem => cartItem.product == batch.product);
            if (item == null)
            {
                item = new CartItem(batch.product);
                productList.Add(item);
            }
            item.AddBatch(batch);
        }

        public int CalculateSum()
        {
            int sum = 0;
            foreach (var product in productList)
            {
                sum += product.totalSum;
            }
            return sum;
        }

        public void Clear()
        {
            productList.Clear();
        }
    }
}
