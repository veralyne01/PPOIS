using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public abstract class NaturalArea
    {
        public double size { get; private set; }
        public string? name { get; private set; }
        public PollutionLevel pollution {  get; protected set; }
        public List<string>? allowedOrganisms;
        public List<Organism>? organisms; 

        public NaturalArea(double size, string? name, List<string>? allowedOrganisms, List<Organism>? organisms)
        {
            this.size = size;
            this.name = name;
            this.allowedOrganisms = allowedOrganisms;
            this.organisms = organisms;
            pollution = new PollutionLevel();
        }

        public void AddCreature(Organism organism)
        {
            if (!allowedOrganisms!.Contains(organism.name!))
            {
                throw new UnsuitableNatureAreaException();
            }
            organisms!.Add(organism);
        }

    }
}
