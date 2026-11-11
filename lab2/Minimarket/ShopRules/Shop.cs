using Minimarket.Staff;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class Shop
    {
        int id;
        List<Employee> employees;
        CashRegister[] cashRegisters = new CashRegister[4];
        public Shop()
        {
            employees = new List<Employee>();

            for (int i = 0; i < cashRegisters.Length; i++)
            {
                cashRegisters[i] = new CashRegister();
            }
        }
    }
}
