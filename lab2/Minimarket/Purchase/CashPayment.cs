using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class CashPayment : Payment
    {
        public int receivedSum { get; }
        public int change { get; }
        public override string method => "Cash";

        public CashPayment(int sum, int receivedSum, int change)
            : base(sum)
        {
            this.receivedSum = receivedSum;
            this.change = change;
        }
    }
}
