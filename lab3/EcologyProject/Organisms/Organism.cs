using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    class Organism
    {
        public string? type { get; private set; }
        public int id { get; private set; }
        private static int nextId = 0;
        public Organism(string type)
        {
            this.type = type;
            id = ++nextId;
        }
    }
}
