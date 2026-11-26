using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public class Lake : NaturalArea
    {
        private const string[] allowed = { };
        Lake(double size, string? name, List<string>? allowedOrganisms, List<Organism>? organisms)
            :base(size, name, allowedOrganisms, organisms)
        {
            pollution.Add(PollutionType.Water);
        }
    }
}
