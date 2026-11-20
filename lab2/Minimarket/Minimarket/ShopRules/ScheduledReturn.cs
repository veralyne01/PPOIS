namespace Minimarket
{
    public class ScheduledReturn
    {
        public Receipt Receipt { get; }
        public Product Product { get; }
        public int Quantity { get; }
        public string Reason { get; }
        public int DueStep { get; private set; }
        public bool IsProcessed { get; private set; }

        public ScheduledReturn(
            Receipt receipt,
            Product product,
            int quantity,
            string reason,
            int dueStep)
        {
            Receipt = receipt ?? throw new ArgumentNullException(nameof(receipt));
            Product = product ?? throw new ArgumentNullException(nameof(product));

            if (quantity <= 0 || quantity > receipt.GetReturnableQuantity(product))
                throw new InvalidReturnException("Invalid scheduled return quantity!");
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Return reason cannot be empty!");
            if (dueStep < 0)
                throw new ArgumentOutOfRangeException(nameof(dueStep));

            Quantity = quantity;
            Reason = reason;
            DueStep = dueStep;
        }

        public bool IsDue(int currentStep)
        {
            return !IsProcessed && currentStep >= DueStep;
        }

        public void Postpone(int steps)
        {
            if (IsProcessed)
                throw new InvalidOperationException("A processed return cannot be postponed!");
            if (steps <= 0)
                throw new ArgumentOutOfRangeException(nameof(steps));

            DueStep += steps;
        }

        public void MarkProcessed()
        {
            if (IsProcessed)
                throw new InvalidOperationException("The return has already been processed!");
            IsProcessed = true;
        }
    }
}
