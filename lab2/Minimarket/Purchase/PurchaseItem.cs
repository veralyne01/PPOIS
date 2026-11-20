using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class PurchaseItem
    {
        public Product product { get; }
        public int number { get; }
        public int price { get; }
        public int discount { get; }
        public int totalSum => (price - discount) * number;

        public PurchaseItem(Product product, int number, int discount = 0)
        {
            this.product = product;
            this.number = number;
            price = product.price;
            this.discount = discount;
        }
    }
}
