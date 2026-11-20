using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class BankCard : Card
    {
        public int money { get; private set; }
        public BankCard()
            :base()
        {
            money = Random.Shared.Next(0, 100000);
        }
    }
}
