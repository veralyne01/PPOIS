using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public class EmptySanctuaryException : ArgumentException
    {
        public EmptySanctuaryException(string name)
            : base("Sanctuary must protect at least one species", name)
        {

        }
    }
}
