using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public abstract class Payment
    {
        int id;
        Purchase purchase;
        double sum;
        DateOnly date;
        bool status;
    }
}
