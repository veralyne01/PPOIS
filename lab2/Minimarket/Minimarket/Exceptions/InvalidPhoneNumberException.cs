namespace Minimarket
{
    public class InvalidPhoneNumberException : Exception
    {
        public InvalidPhoneNumberException()
            : base("Wrong phone number!")
        {

        }
    }
}
