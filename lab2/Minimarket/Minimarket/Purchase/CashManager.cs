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

        public static Dictionary<Nominals, int> ManageNominals(Dictionary<Nominals, int> dict, int change)
        {
            if (change < 0)
                throw new ArgumentException("Change must not be negative!");

            var result = new Dictionary<Nominals, int>();
            if (change == 0)
                return result;

           
            var available = dict
                .Where(p => p.Value > 0)
                .OrderByDescending(p => (int)p.Key)
                .Select(p => (Nominal: p.Key, Count: p.Value))
                .ToList();

            var taken = new int[available.Count];
            var failed = new HashSet<(int index, int remaining)>();

            bool Solve(int index, int remaining)
            {
                if (remaining == 0)
                    return true;
                if (index == available.Count)
                    return false;
                if (failed.Contains((index, remaining)))
                    return false;

                int value = (int)available[index].Nominal;
                int max = Math.Min(available[index].Count, remaining / value);

                for (int k = max; k >= 0; k--)
                {
                    taken[index] = k;
                    if (Solve(index + 1, remaining - k * value))
                        return true;
                }

                taken[index] = 0;
                failed.Add((index, remaining));
                return false;
            }

            if (!Solve(0, change))
                throw new InvalidChangeException();

            for (int i = 0; i < available.Count; i++)
            {
                if (taken[i] > 0)
                    result[available[i].Nominal] = taken[i];
            }

            return result;
        }
    }
}
