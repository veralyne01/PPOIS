namespace Minimarket
{
    public class ExpiredProductException : Exception
    {
        public ExpiredProductException()
            : base("The product batch has expired!")
        {
        }

        public ExpiredProductException(string message)
            : base(message)
        {
        }
    }
}
