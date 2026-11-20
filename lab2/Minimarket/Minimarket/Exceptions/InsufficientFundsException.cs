using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException()
            : base("The customer has nothing to pay with!")
        {

        }

        public InsufficientFundsException(string message)
            : base(message)
        {

        }
    }
}
