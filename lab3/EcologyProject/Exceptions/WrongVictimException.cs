using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{  
    public class WrongVictimException : Exception
    {
        public WrongVictimException()
            : base("The hunter killed the wrong creature")
        {

        }
    }
}
