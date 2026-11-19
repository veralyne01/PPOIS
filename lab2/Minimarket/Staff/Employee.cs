using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public abstract class Employee
    {
        public int Id { get; }
        private static int nextEmployeeId = 500;
        public string Name { get; }

        protected Employee(string name)
        {
            Id = ++nextEmployeeId;
            Name = name;
        }
    }
}
