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
        private double balance;
        private bool isFree;
        private bool isOpen;
        private Dictionary<Nominals, int> nominals;
        public CashRegister()
        {
            id = ++nextCashId;
            isFree = true;
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

        public void GiveChange(CashPayment payment)
        {
            double sum = CashManager.Sum(nominals);
            if (sum < payment.receivedSum) throw new InvalidChangeException();
           
        }
    }
}
