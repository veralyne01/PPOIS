namespace Minimarket
{
    public class CardPayment : Payment
    {
        public BankCard card { get; }
        public override string method => "Card";

        public CardPayment(int sum, BankCard card)
            : base(sum)
        {
            this.card = card;
        }
    }
}
