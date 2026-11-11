using Minimarket.Staff;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class Purchase
    {
        int id;
        Customer customer;
        Cashier cashier;
        DateOnly date;
        List<PurchaseItem> items;
    }
}
