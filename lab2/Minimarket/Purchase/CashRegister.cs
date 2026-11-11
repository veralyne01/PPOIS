using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class CashRegister
    {
        int id;
        Cashier cashier;
        double balance;
        bool status;
        Dictionary<Nominals, int> nominals;
    }
}
