using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public class PollutionLevel
    {
        private Dictionary<PollutionType, double> levels = new();

        public void Add(PollutionType type, double value = 0)
        {
            levels[type] = value;
        }

        public bool Increase(PollutionType type, double value)
        {
            if (!levels.ContainsKey(type))
                return false;

            levels[type] += value;
            return true;
        }

        public double? GetLevel(PollutionType type)
        {
            if (!levels.TryGetValue(type, out double value))
                return null;

            return value;
        }

        public bool HasParameter(PollutionType type)
        {
            return levels.ContainsKey(type);
        }
    }
}
