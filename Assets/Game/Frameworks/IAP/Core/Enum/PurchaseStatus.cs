namespace com.foundation.iap.core
{
    public enum PurchaseStatus
    {
        /// <summary>
        /// Purchasing may be disabled in security settings.
        /// </summary>
        PurchasingUnavailable = 0,

        /// <summary>
        /// Another purchase is already in progress.
        /// </summary>
        ExistingPurchasePending = 1,

        /// <summary>
        /// The product was reported unavailable by the purchasing system.
        /// </summary>
        ProductUnavailable = 2,

        /// <summary>
        /// Signature validation of the purchase's receipt failed.
        /// </summary>
        SignatureInvalid = 3,

        /// <summary>
        /// The user opted to cancel rather than proceed with the purchase.
        /// This is not specified on platforms that do not distinguish cancellation from other failure (Amazon, Microsoft).
        /// </summary>
        UserCancelled = 4,

        /// <summary>
        /// There was a problem with the payment. This is unique to Apple platforms.
        /// </summary>
        PaymentDeclined = 5,

        /// <summary>
        /// The transaction has already been completed successfully.
        /// This error can occur on Apple platforms if the transaction is finished successfully while the user is logged out of the app store, using a receipt generated while the user was logged in.
        /// </summary>
        DuplicateTransaction = 6,

        /// <summary>
        /// A catch all for remaining purchase problems.
        /// Note: Use Enum.Parse to use this named constant if targeting Unity 5.3 or 5.4.
        /// Its value differs for 5.5+ which introduced DuplicateTransaction.
        /// </summary>
        Unknown = 7,
        
        Unauthorized,
        
        InvalidRequest,
        
        InvalidAppId,
        
        ReceiptValidationFailed,
        
        TransactionNotMatch,
        
        UnsupportedStore,
        
        UnexpectedResponse,
        
        RequestTimeout,
    }
}