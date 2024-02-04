namespace com.foundation.iap.core
{
    public interface IProduct
    {
        string Id { get; }
        PurchaseType Type { get; }
        decimal LocalizedPrice { get; }
        string IsoCurrencyCode { get; }
        string LocalizedPriceString { get; }
        string LocalizedTitle { get; }
        string LocalizedDescription { get; }
        bool HasReceipt { get; }
        string Receipt { get; }
    }
}