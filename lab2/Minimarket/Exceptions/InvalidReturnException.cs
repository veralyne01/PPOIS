using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class InvalidReturnException : Exception
    {
        public InvalidReturnException()
            : base("Invalid return!")
        {

        }

        public InvalidReturnException(string message)
            : base(message)
        {

        }
    }
}
