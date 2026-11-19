using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class InvalidChangeException : Exception
    {
        public InvalidChangeException()
        : base("Not enough money for change!")
        {

        }
    }
}
