using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Minimarket
{
    public class CashRegister
    {
        private int id;
        private static int nextCashId = 300;
        private Cashier? cashier = null;
        private bool isOpen;
        private Dictionary<Nominals, int> nominals;
        private readonly DiscountCalculator discountCalculator;

  
        private readonly Queue<Customer> queue = new();
        private Customer? currentCustomer = null;
        private int paymentTimer;

      
        private readonly List<Purchase> purchases = new();

        public int Id => id;
        public bool IsOpen => isOpen;
        public Cashier? AssignedCashier => cashier;
        public int QueueLength => queue.Count + (currentCustomer != null ? 1 : 0);
        public bool IsFree => currentCustomer == null && queue.Count == 0;
        public long Balance => CashManager.Sum(nominals);
        public IReadOnlyList<Purchase> Purchases => purchases;

        public CashRegister(DiscountCalculator? discountCalculator = null)
        {
            this.discountCalculator = discountCalculator ?? new DiscountCalculator();
            id = ++nextCashId;
            nominals = new Dictionary<Nominals, int>(){
                { Nominals.OnePound, 10 },
                { Nominals.TwoPounds, 10 },
                { Nominals.FivePounds, 10 },
                { Nominals.TenPounds, 7 },
                { Nominals.TwentyPounds, 5 },
                { Nominals.FiftyPounds, 5 },
                { Nominals.OneDollar, 10 },
                { Nominals.FiveDollars, 7 },
                { Nominals.TenDollars, 5 },
                { Nominals.TwentyDollars, 3 },
                { Nominals.FiftyDollars, 2 },
                { Nominals.OneHundrendDollars, 1 },
                { Nominals.TwoHundredDollars, 10 }
            };
        }

        public void AssignCashier(Cashier cashier)
        {
            if (this.cashier != null)
                throw new InvalidOperationException(
                    "A cashier is already assigned!");
            this.cashier = cashier;
        }

        public void Open()
        {
            if (cashier == null)
                throw new InvalidOperationException(
                    "Cannot open a register without a cashier!");
            isOpen = true;
        }

        public Dictionary<Nominals, int> GiveChange(int change)
        {
            Dictionary<Nominals, int> notes = CashManager.ManageNominals(nominals, change);
            foreach (var pair in notes)
            {
                CashManager.Remove(nominals, pair.Key, pair.Value);
            }
            return notes;
        }

        public void Enqueue(Customer customer)
        {
            if (!isOpen)
                throw new InvalidOperationException("The register is closed!");
            queue.Enqueue(customer);
            customer.SetStatus(Status.InLineAtCheckout);
        }
        public Customer? NextStep(out Receipt? receipt)
        {
            receipt = null;
            if (!isOpen)
                return null;

            if (currentCustomer == null)
            {
                if (queue.Count == 0)
                {
                    return null;
                }
             
                currentCustomer = queue.Dequeue();
                currentCustomer.SetStatus(Status.Paying);
                paymentTimer = Random.Shared.Next(1, 4);
                Console.WriteLine($"Register {id}: {currentCustomer.name} is paying");
                return null;
            }

            paymentTimer--;
            if (paymentTimer > 0)
                return null;

            Customer customer = currentCustomer;
            currentCustomer = null;
            try
            {
                receipt = Checkout(customer);
                Purchase purchase = receipt.purchase;
                Console.WriteLine(
                    $"Register {id}: {customer.name} paid {purchase.total - purchase.bonusesSpent} " +
                    $"({purchase.payment.method}), bonuses spent: {purchase.bonusesSpent}, " +
                    $"accrued: {purchase.bonusesAccrued}");
            }
            catch (Exception ex) when (
                ex is InsufficientFundsException ||
                ex is InvalidChangeException ||
                ex is InvalidPaymentException)
            {
                Console.WriteLine($"Register {id}: {customer.name} could not pay - {ex.Message}");
            }
            return customer;
        }

        private Receipt Checkout(Customer customer)
        {
            if (cashier == null)
                throw new InvalidOperationException(
                    "Cannot serve customers without a cashier!");

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int total = customer.cart.CalculateSum()
                - discountCalculator.CalculateTotalDiscount(customer.cart, today);
            if (total <= 0)
                throw new InvalidPaymentException("The cart is empty!");

            int bonusesSpent = 0;
            LoyalityCard? loyalityCard = customer.loyalityCard;
            if (loyalityCard != null && loyalityCard.bonuses * 10 > total)
                bonusesSpent = Math.Min(loyalityCard.bonuses, total);
            int toPay = total - bonusesSpent;

            bool canPayCash = customer.hasWallet && customer.wallet!.sum >= toPay;
            bool canPayCard = customer.hasBankCard && customer.card!.money >= toPay;
            if (!canPayCash && !canPayCard)
                throw new InsufficientFundsException(
                    "Not enough money to pay for the purchase!");

            Payment payment;
            if (canPayCard && (!canPayCash || Random.Shared.Next(2) == 0))
            {
                payment = PayByCard(customer, toPay);
            }
            else
            {
                try
                {
                    payment = PayByCash(customer, toPay);
                }
                catch (InvalidChangeException) when (canPayCard)
                {
                    Console.WriteLine($"Register {id}: no change available, paying by card");
                    payment = PayByCard(customer, toPay);
                }
            }

      
            int bonusesAccrued = 0;
            if (loyalityCard != null)
            {
                bonusesAccrued = total / 100;
                if (bonusesSpent > 0)
                    loyalityCard.SpendBonuses(bonusesSpent);
                if (bonusesAccrued > 0)
                    loyalityCard.AccrueBonuses(bonusesAccrued);
            }

            List<PurchaseItem> items = new();
            foreach (CartItem item in customer.cart.ProductList)
            {
                int discount = discountCalculator.CalculateDiscountPerUnit(item.product, today);
                items.Add(new PurchaseItem(item.product, item.number, discount));
            }
            Purchase purchase = new Purchase(
                customer,
                cashier,
                items,
                payment,
                bonusesSpent,
                bonusesAccrued
            );
            purchases.Add(purchase);
            return new Receipt(purchase);
        }

        private Payment PayByCard(Customer customer, int sum)
        {
            customer.card!.Withdraw(sum);
            return new CardPayment(sum, customer.card);
        }

        private Payment PayByCash(Customer customer, int sum)
        {
         
            Dictionary<Nominals, int> given = customer.wallet!.PickNominals(sum);
            int givenSum = (int)CashManager.Sum(given);
            int change = givenSum - sum;

       
            foreach (var pair in given)
            {
                CashManager.Add(nominals, pair.Key, pair.Value);
            }

            Dictionary<Nominals, int> changeNotes;
            try
            {
                changeNotes = GiveChange(change);
            }
            catch (InvalidChangeException)
            {
                foreach (var pair in given)
                {
                    CashManager.Remove(nominals, pair.Key, pair.Value);
                }
                throw;
            }

            customer.wallet.Remove(given);
            customer.wallet.Add(changeNotes);
            return new CashPayment(sum, givenSum, change);
        }
    }
}
