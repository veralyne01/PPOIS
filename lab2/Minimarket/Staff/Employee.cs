using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public abstract class Employee
    {
        public int number { get; }
        public string Name { get; }
        private static int nextEmployeeNumber = 600;

        protected Employee(string name)
        {
            number = ++nextEmployeeNumber;
            Name = name;
        }
    }
}
