using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class LoyalityCard : Card
    {
        public int bonuses { get; set; }
        public LoyalityCard()
            :base()
        {
            bonuses = Random.Shared.Next(0, 100);
        }
    }
}
