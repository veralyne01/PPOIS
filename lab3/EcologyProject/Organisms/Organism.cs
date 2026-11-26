using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public abstract class Organism
    {
        public string? type { get; private set; }
        public int id { get; private set; }
        private static int nextId = 0;
        public string? name { get; private set; }
        public List<string>? food { get; protected set; }
        public Organism(string type, string name)
        {
            this.type = type;
            id = ++nextId;
            this.name = name;
        }
    }
}
