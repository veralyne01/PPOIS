using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class CartItem
    {
        private Product product;
        public CartItem(Product product)
        {
            this.product = product;
        }
        public int number;
        public int totalSum => product.price * number;
        public int IncreaseNumber() { return number++; }
        public int DecreaseNumber() { return number--; }
    }
}
