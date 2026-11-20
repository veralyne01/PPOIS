using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Minimarket
{
    public class Product : Item
    {
        public int price;
        int barcode;
        void ChangePrice(int price)
        {
            this.price = price;
        }
    }
}
