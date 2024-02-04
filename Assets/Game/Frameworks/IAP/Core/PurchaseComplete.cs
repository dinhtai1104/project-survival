namespace com.foundation.iap.core
{
    [System.Serializable]
    public struct PurchaseComplete
    {
        public string ProductId { get; }
        public string Receipts { get; }

        public PurchaseComplete(string productId, string receipts)
        {
            ProductId = productId;
            Receipts = receipts;
        }
    }
}