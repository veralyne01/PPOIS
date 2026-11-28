using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public class MapException : Exception
    {
        public MapException()
            : base("This map cell is already occupied.")
        {

        }
    }
}
