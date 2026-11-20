namespace Minimarket
{
    public class CustomerWallet
    {
        public int sum { get; private set; }
        public Dictionary<Nominals, int> nominals { get; private set; } = new Dictionary<Nominals, int>();

        public CustomerWallet()
        {
            foreach (Nominals nominal in Enum.GetValues<Nominals>())
            {
                if ((int)nominal > 2000)
                    continue;
                int count = Random.Shared.Next(0, 3);
                if (count > 0)
                    CashManager.Add(nominals, nominal, count);
            }
            sum = (int)CashManager.Sum(nominals);
        }

        public Dictionary<Nominals, int> PickNominals(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount must not be negative!");
            if (amount > sum)
                throw new InsufficientFundsException("Not enough cash in the wallet!");

            Dictionary<Nominals, int> result = new();
            int remaining = amount;

            foreach (var pair in nominals.OrderByDescending(p => (int)p.Key))
            {
                int count = Math.Min(pair.Value, remaining / (int)pair.Key);
                if (count > 0)
                {
                    result[pair.Key] = count;
                    remaining -= count * (int)pair.Key;
                }
            }

            if (remaining > 0)
            {
                var extra = nominals
                    .Where(p =>
                        (int)p.Key >= remaining &&
                        p.Value - CashManager.GetCount(result, p.Key) > 0)
                    .OrderBy(p => (int)p.Key)
                    .First();
                CashManager.Add(result, extra.Key, 1);
            }
            return result;
        }

        public void Remove(Dictionary<Nominals, int> notes)
        {
            foreach (var pair in notes)
            {
                if (!CashManager.Remove(nominals, pair.Key, pair.Value))
                    throw new InvalidOperationException(
                        "The wallet does not contain these notes!");
            }
            sum = (int)CashManager.Sum(nominals);
        }

        public void Add(Dictionary<Nominals, int> notes)
        {
            foreach (var pair in notes)
            {
                CashManager.Add(nominals, pair.Key, pair.Value);
            }
            sum = (int)CashManager.Sum(nominals);
        }
    }
}
