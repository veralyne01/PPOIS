using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Minimarket
{
    public class Customer
    {
        public int id { get; private set; }
        private static int nextCustomerId = 1000;
        public string name;
        PhoneNumber number;
        DateOnly dateOfRegistration;
        ShoppingCart cart;
        public CustomerWallet? wallet { get; private set; }
        public BankCard? card { get; private set; }
        public LoyalityCard? loyalityCard { get; private set; }
        public bool hasBankCard { get; private set; }
        public bool hasWallet { get; private set; }
        public Status status { get; private set; }
        public Customer(string name)
        {
            id = ++nextCustomerId;
            this.name = name;
            status = Status.LookingForProducts;
            loyalityCard = new();
            int randomizer = Random.Shared.Next(0,3);
            switch (randomizer)
            {
                case 0:
                    wallet = new();
                    hasWallet = true;
                    break;
                case 1:
                    card = new();
                    hasBankCard = true;
                    break;
                case 2:
                    wallet = new();
                    hasWallet = true;
                    card = new();
                    hasBankCard = true;
                    break;
                default:
                    throw new InsufficientFundsException();
            }
        }
    }
}
