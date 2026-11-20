using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class Customer
    {
        public int id { get; private set; }
        private static int nextCustomerId = 1000;
        public string name;
        PhoneNumber number;
        DateOnly dateOfRegistration;
        public Status status { get; private set; }
        public Customer(string name)
        {
            id = ++nextCustomerId;
            this.name = name;
            status = Status.LookingForProducts;
        }
    }
}
