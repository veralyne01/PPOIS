using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class Receipt
    {
        int id;
        PurchaseItem item;
        DateOnly date;
        Cashier cashier;
        double sum;
    }
}
