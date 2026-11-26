using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public class UnsuitableNatureAreaException : Exception
    {
        public UnsuitableNatureAreaException()
            :base("The area is unsuitable for the organism")
        {

        }
    }
}
