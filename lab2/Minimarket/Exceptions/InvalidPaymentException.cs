using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class InvalidPaymentException : Exception
    {
        public InvalidPaymentException()
            : base("Invalid payment!")
        {

        }

        public InvalidPaymentException(string message)
            : base(message)
        {

        }
    }
}
