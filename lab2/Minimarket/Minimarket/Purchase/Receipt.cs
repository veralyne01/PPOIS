using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class Receipt
    {
        private static int nextReceiptId = 0;
        public int id { get; }
        public Purchase purchase { get; }
        public DateOnly date => purchase.date;
        public Cashier cashier => purchase.cashier;
        public int sum => purchase.total;
        public IReadOnlyList<PurchaseItem> Items => purchase.Items;
        public Payment payment => purchase.payment;

        private readonly Dictionary<Product, int> returned = new();

        public Receipt(Purchase purchase)
        {
            id = ++nextReceiptId;
            this.purchase = purchase;
        }

        public int GetReturnableQuantity(Product product)
        {
            PurchaseItem? item = Items.FirstOrDefault(i => i.product == product);
            if (item == null)
                return 0;
            return item.number - returned.GetValueOrDefault(product);
        }

        public void MarkReturned(Product product, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (quantity <= 0 || quantity > GetReturnableQuantity(product))
                throw new InvalidReturnException("Invalid quantity to mark as returned!");

            returned[product] = returned.GetValueOrDefault(product) + quantity;
        }
    }
}
