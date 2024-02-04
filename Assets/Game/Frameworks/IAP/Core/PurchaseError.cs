namespace com.foundation.iap.core
{
    [System.Serializable]
    public struct PurchaseError
    {
        public string ProductId { get; }
        public PurchaseStatus ErrorType { get; }

        public PurchaseError(string productId, PurchaseStatus errorType)
        {
            ProductId = productId;
            ErrorType = errorType;
        }
    }
}