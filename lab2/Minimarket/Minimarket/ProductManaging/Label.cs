namespace Minimarket
{
    public class Label
    {
        public string Barcode { get; }
        public DateOnly ExpirationDate { get; }
        public int BatchId { get; }
        public ProductBatch Batch { get; }

        public Label(ProductBatch batch)
        {
            Batch = batch ?? throw new ArgumentNullException(nameof(batch));
            Barcode = batch.product.barcode;
            ExpirationDate = batch.expirationDate;
            BatchId = batch.Id;

            if (string.IsNullOrWhiteSpace(Barcode) || !Barcode.All(char.IsDigit))
                throw new InvalidBarcodeException();
        }
    }
}
