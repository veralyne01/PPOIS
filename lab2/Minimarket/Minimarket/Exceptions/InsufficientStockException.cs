namespace Minimarket
{
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException()
            : base("Not enough products!")
        {

        }
    }
}
