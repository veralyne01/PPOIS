using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public class ObjectIsNotInTheEcosystemException : Exception
    {
        public ObjectIsNotInTheEcosystemException()
            : base("The object must belong to this ecosystem")
        {

        }
    }
}
