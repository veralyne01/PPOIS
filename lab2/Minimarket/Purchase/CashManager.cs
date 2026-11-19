using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket { 
    static public class CashManager
    {

        public static void Add(Dictionary<Nominals, int> dict, Nominals denomination, int count)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be positive!");

            if (dict.ContainsKey(denomination))
                dict[denomination] += count;
            else
                dict[denomination] = count;
        }

        public static bool Remove(Dictionary<Nominals, int> dict, Nominals denomination, int count)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be positive!");

            if (!dict.ContainsKey(denomination) ||
                dict[denomination] < count)
                return false;

            dict[denomination] -= count;

            if (dict[denomination] == 0)
                dict.Remove(denomination);

            return true;
        }
        public static int GetCount(Dictionary<Nominals, int> dict, Nominals denomination)
        {
            return dict.GetValueOrDefault(denomination, 0);
        }

        public static long Sum(Dictionary<Nominals, int> dict)
        {
            long total = 0;

            foreach (var item in dict)
            {
                total += (int)item.Key * item.Value;
            }

            return total;
        }
    }
}
