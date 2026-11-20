using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException()
            : base("Not enough products!")
        {

        }
    }
}
