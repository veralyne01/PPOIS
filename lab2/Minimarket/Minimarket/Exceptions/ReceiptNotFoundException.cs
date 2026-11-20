namespace Minimarket
{
    public class ReceiptNotFoundException : Exception
    {
        public ReceiptNotFoundException()
            : base("Receipt not found!")
        {
        }

        public ReceiptNotFoundException(string message)
            : base(message)
        {
        }
    }
}
