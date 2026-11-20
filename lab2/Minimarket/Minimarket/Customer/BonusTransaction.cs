namespace Minimarket
{
    public class BonusTransaction
    {
        private static int nextId = 0;
        public int id { get; }
        public int bonuses { get; }
        public DateOnly date { get; }
        public LoyalityCard card { get; }
        public OperationType operation { get; }

        public BonusTransaction(LoyalityCard card, int bonuses, OperationType operation)
        {
            id = ++nextId;
            this.card = card;
            this.bonuses = bonuses;
            this.operation = operation;
            date = DateOnly.FromDateTime(DateTime.Today);
        }
    }
}
