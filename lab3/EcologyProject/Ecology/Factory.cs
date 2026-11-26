using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public class Factory
    {
        public int id { get; private set; }
        public string? name { get; private set; }
        private int nextFactoryId = 0;
        FactoryType type;
        Factory()
    }
}
