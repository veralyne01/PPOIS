namespace Minimarket
{
    public class BankCard : Card
    {
        public int money { get; private set; }

        public BankCard()
            : base()
        {
            money = Random.Shared.Next(0, 100000);
        }

        public void Withdraw(int sum)
        {
            if (sum < 0)
                throw new ArgumentException("Sum must not be negative!");
            if (sum > money)
                throw new InvalidPaymentException("Not enough money on the card!");
            money -= sum;
        }

        public void Deposit(int sum)
        {
            if (sum < 0)
                throw new ArgumentException("Sum must not be negative!");
            money += sum;
        }
    }
}
