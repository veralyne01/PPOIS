using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class ShopAlreadyOpenedException : Exception
    {
        public ShopAlreadyOpenedException() 
            : base("The shop is already opened!")
        {

        }
    }
}
