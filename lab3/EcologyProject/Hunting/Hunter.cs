using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    class Hunter
    {
        public Dictionary<Organism, int>? target { get; private set; }
        public Dictionary<Organism, int>? killed { get; private set; }
        public Hunter(Dictionary<Organism, int>? target)
        {
            this.target = target;
            killed = new Dictionary<Organism, int>();
        }
        public void Kill(Organism victim)
        {
            if (!target!.ContainsKey(victim))
            {
                throw new
            }
        }
    }
}
