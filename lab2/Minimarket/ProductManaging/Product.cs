using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Minimarket
{
    class Product : Item
    {
        double price;
        int barcode;
        void ChangePrice(double price)
        {
            this.price = price;
        }
    }
}
