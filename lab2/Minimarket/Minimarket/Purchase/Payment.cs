using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public abstract class Payment
    {
        private static int nextPaymentId = 0;
        public int id { get; }
        public int sum { get; }
        public DateOnly date { get; }
        public bool status { get; }
        public abstract string method { get; }

        protected Payment(int sum)
        {
            if (sum <= 0)
                throw new InvalidPaymentException("Payment amount must be positive!");

            id = ++nextPaymentId;
            this.sum = sum;
            date = DateOnly.FromDateTime(DateTime.Today);
            status = true;
        }
    }
}
