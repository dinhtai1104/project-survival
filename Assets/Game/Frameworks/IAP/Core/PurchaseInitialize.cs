namespace com.foundation.iap.core
{
    [System.Serializable]
    public struct PurchaseInitialize
    {
        public InitializationStatus Status { get; }

        public PurchaseInitialize(InitializationStatus status)
        {
            Status = status;
        }
    }
}