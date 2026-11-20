namespace Minimarket
{
    public class InvalidBarcodeException : Exception
    {
        public InvalidBarcodeException()
            : base("The barcode must contain digits only!")
        {
        }

        public InvalidBarcodeException(string message)
            : base(message)
        {
        }
    }
}
