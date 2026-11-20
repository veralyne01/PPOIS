using System;
using System.Collections.Generic;

namespace Minimarket
{
    public class Shop
    {
        private readonly List<Department> departments = new();
        private readonly List<Employee> employees = new();
        private readonly List<CashRegister> cashRegisters = new();
        private readonly List<Customer> customers = new();

        private bool isOpen = false;

        public IReadOnlyList<Department> Departments => departments;
        public IReadOnlyList<Employee> Employees => employees;
        public IReadOnlyList<CashRegister> CashRegisters => cashRegisters;
        public IReadOnlyList<Customer> Customers => customers;

        public bool IsOpen => isOpen;

        public Shop()
        {
            CreateDepartments();
            CreateCashiers();
            CreateSpecialists();
        }

        private void CreateDepartments()
        {
            string[] departmentNames =
            {
                "Dairy",
                "Meat and fish",
                "Bread and cereal",
                "Fruit and vegetables",
                "Sweets"
            };

            foreach (string name in departmentNames)
            {
                departments.Add(new Department(name));
            }
        }

        private void CreateCashiers()
        {
            for (int i = 0; i < 4; i++)
            {
                Cashier cashier = new Cashier(
                    $"Cashier {i + 1}"
                );

                CashRegister register = new CashRegister();

                register.AssignCashier(cashier);

                employees.Add(cashier);
                cashRegisters.Add(register);
            }
        }

        private void CreateSpecialists()
        {
            employees.Add(new InventoryManager("Inventory Manager"));
            employees.Add(new ReturnManager("Return Manager"));
        }

        public void AddCustomer(Customer customer)
        {
            customers.Add(customer);
        }

        public void Open()
        {
            if (isOpen)
                throw new ShopAlreadyOpenedException();

            foreach (CashRegister register in cashRegisters)
            {
                register.Open();
            }

            isOpen = true;

            Console.WriteLine("Shop successfully opened!");
        }
    }
}