using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class ShoppingCart
    {
        private int id;
        private static int nextCartId = 0;
        ShoppingCart()
        {
            id = ++nextCartId;
        }
        List<CartItem>? productList; 
        public int CalculateSum()
        {
            int sum = 0;
            foreach(var product in productList)
            {
                sum += product.totalSum;
            }
            return sum;
        }
    }
}
