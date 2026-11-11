using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class Supply
    {
        int id;
        Supplier supplier;
        DateOnly date;
        List<Product> products;
        double averageSum;
        bool status;
    }
}
