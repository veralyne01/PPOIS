using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class ReturnManager : Employee
    {
        public ReturnManager(string name)
        : base(name)
        {
        }

        public Return ProcessReturn(
            Receipt receipt,
            Product product,
            int quantity,
            string? reason,
            IReadOnlyList<CashRegister> registers)
        {
            PurchaseItem? item = receipt.Items.FirstOrDefault(i => i.product == product);
            if (item == null)
                throw new ProductNotFoundException("There is no such product in the receipt!");
            if (quantity <= 0 || quantity > receipt.GetReturnableQuantity(product))
                throw new InvalidReturnException("Invalid quantity to return!");

            int sum = item.price * quantity;
            Customer customer = receipt.purchase.customer;

            if (receipt.payment is CardPayment)
            {
   
                customer.card!.Deposit(sum);
            }
            else
            {
           
                Dictionary<Nominals, int>? notes = null;
                foreach (CashRegister register in registers)
                {
                    try
                    {
                        notes = register.GiveChange(sum);
                        break;
                    }
                    catch (InvalidChangeException)
                    {
                    }
                }
                if (notes == null)
                    throw new InvalidChangeException();
                customer.wallet!.Add(notes);
            }

            receipt.MarkReturned(product, quantity);
            return new Return(receipt, product, quantity, sum, reason);
        }
    }
}
