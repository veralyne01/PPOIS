using System.Linq;

namespace Minimarket
{
    public class ReturnManager : Employee
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
            if (receipt == null)
                throw new ReceiptNotFoundException();
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (registers == null)
                throw new ArgumentNullException(nameof(registers));

            PurchaseItem? item = receipt.Items.FirstOrDefault(i => i.product == product);
            if (item == null)
                throw new ProductNotFoundException("There is no such product in the receipt!");
            if (quantity <= 0 || quantity > receipt.GetReturnableQuantity(product))
                throw new InvalidReturnException("Invalid quantity to return!");

            int sum = (item.price - item.discount) * quantity;
            Customer customer = receipt.purchase.customer;

            if (receipt.payment is CardPayment)
            {
                if (customer.card == null)
                    throw new InvalidPaymentException("The original bank card is unavailable!");
                customer.card.Deposit(sum);
            }
            else
            {
                if (customer.wallet == null)
                    throw new InvalidPaymentException("The customer's cash wallet is unavailable!");

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

                customer.wallet.Add(notes);
            }

            receipt.MarkReturned(product, quantity);
            return new Return(receipt, product, quantity, sum, reason);
        }
    }
}
