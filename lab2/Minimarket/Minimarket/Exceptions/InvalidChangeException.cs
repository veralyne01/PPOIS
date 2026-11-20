namespace Minimarket
{
    public class InvalidChangeException : Exception
    {
        public InvalidChangeException()
            : base("Not enough money for change!")
        {
        }

        public InvalidChangeException(string message)
            : base(message)
        {
        }
    }
}
