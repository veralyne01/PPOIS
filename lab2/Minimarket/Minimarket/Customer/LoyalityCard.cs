using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class LoyalityCard : Card
    {
        public int bonuses { get; set; }
        private readonly List<BonusTransaction> transactions = new();
        public IReadOnlyList<BonusTransaction> Transactions => transactions;

        public LoyalityCard()
            :base()
        {
            bonuses = Random.Shared.Next(0, 100);
        }

        public void SpendBonuses(int amount)
        {
            if (amount <= 0 || amount > bonuses)
                throw new ArgumentException("Invalid bonuses amount!");
            bonuses -= amount;
            transactions.Add(new BonusTransaction(this, amount, OperationType.Purchase));
        }

        public void AccrueBonuses(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Invalid bonuses amount!");
            bonuses += amount;
            transactions.Add(new BonusTransaction(this, amount, OperationType.Accrual));
        }
    }
}
