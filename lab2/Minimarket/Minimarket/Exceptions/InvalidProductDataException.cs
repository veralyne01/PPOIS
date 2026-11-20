namespace Minimarket
{
    public class InvalidProductDataException : Exception
    {
        public InvalidProductDataException()
            : base("Invalid product data!")
        {

        }
        public InvalidProductDataException(string message)
            : base()
        {

        }
    }
}
