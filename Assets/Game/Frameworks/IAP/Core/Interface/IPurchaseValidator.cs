namespace com.foundation.iap.core
{
    public interface IPurchaseValidator
    {
        PurchaseState Validate(string productId, string productType, string receipt);
    }
}