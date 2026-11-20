using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public abstract class Card
    {
        public int number { get; private set; }
        private static int nextCardNumber = Random.Shared.Next(100000, 1000000);
        public DateOnly dateOfRegistration { get; private set; }
        public Card()
        {
            number = nextCardNumber++;
            dateOfRegistration = DateManager.GenerateRandomDate();
        }
    }
}
