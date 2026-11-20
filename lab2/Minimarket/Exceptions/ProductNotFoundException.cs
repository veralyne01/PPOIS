using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException()
            : base("Product not found!")
        {

        }

        public ProductNotFoundException(string message)
            : base(message)
        {

        }
    }
}
