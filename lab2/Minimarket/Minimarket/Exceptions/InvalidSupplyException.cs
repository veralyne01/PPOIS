namespace Minimarket
{
    public class InvalidSupplyException : Exception
    {
        public InvalidSupplyException()
            : base("The supply data is invalid!")
        {
        }

        public InvalidSupplyException(string message)
            : base(message)
        {
        }
    }
}
