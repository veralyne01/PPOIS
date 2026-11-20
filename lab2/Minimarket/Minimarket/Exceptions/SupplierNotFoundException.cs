namespace Minimarket
{
    public class SupplierNotFoundException : Exception
    {
        public SupplierNotFoundException()
            : base("Supplier not found!")
        {
        }

        public SupplierNotFoundException(string message)
            : base(message)
        {
        }
    }
}
