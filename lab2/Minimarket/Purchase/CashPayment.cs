using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class CashPayment : Payment
    {
        public int receivedSum;
        public int change;
        public CashPayment(int receivedSum)
        {
            this.receivedSum = receivedSum;
        }
    }
}
