using System;
using System.Collections.Generic;
using com.foundation.iap.core;
// using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace com.foundation.iap.unity
{
    public class UnityPurchase : IPurchase, IDetailedStoreListener
    {
        public event Action<PurchaseInitialize> PurchaseInitialized;
        public event Action<PurchaseError> PurchaseFailed;
        public event Action<PurchaseComplete> PurchaseCompleted;
        public event Action<PurchaseProcess> PurchaseProcessing;
        public event Action<PurchaseComplete> ServerPurchaseValid;

        public bool IsInitialized => _storeController != null && _storeExtensionProvider != null;
        public IEnumerable<IProduct> Products => _products;
        public IPurchaseValidator LocalValidator => _localValidator;
        public IPurchaseValidator ServerValidator => _serverValidator;

        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;
        private readonly IPurchaseValidator _localValidator;
        private readonly IPurchaseValidator _serverValidator;
        private readonly List<UnityProduct> _products;

        public UnityPurchase(IPurchaseValidator localValidator)
        {
            _localValidator = localValidator;
            _products = new List<UnityProduct>();
        }

        public UnityPurchase(IPurchaseValidator localValidator, IPurchaseValidator serverValidator) : this(localValidator)
        {
            _serverValidator = serverValidator;
        }

        public /*async*/ void InitializePurchasing(IEnumerable<ProductData> productData)
        {
            // await UnityServices.InitializeAsync();

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var product in productData)
            {
                builder.AddProduct(product.Id, product.Type.ToUnityProductType());
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _storeExtensionProvider = extensions;

            foreach (var product in _storeController.products.all)
            {
                _products.Add(new UnityProduct(product));
            }

            // PurchaseCompleted += OnPurchaseCompleted;
            PurchaseInitialized?.Invoke(new PurchaseInitialize(InitializationStatus.Ready));
            Debug.Log($"[{nameof(UnityPurchase)}] OnInitialized");
        }

        private void OnPurchaseCompleted(PurchaseComplete purchaseComplete)
        {
            Debug.Log($"[{nameof(UnityPurchase)}] Purchase {purchaseComplete.ProductId}: Confirm Pending.");
            ConfirmPurchase(purchaseComplete.ProductId);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            PurchaseInitialized?.Invoke(new PurchaseInitialize(InitializationStatus.NotReady));
            Debug.Log($"[{nameof(UnityPurchase)}] OnInitializeFailed FailureReason: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            PurchaseInitialized?.Invoke(new PurchaseInitialize(InitializationStatus.NotReady));
            Debug.Log($"[{nameof(UnityPurchase)}] OnInitializeFailed FailureMessage: {message}");
        }

        public void Purchase(string productId)
        {
            if (!IsInitialized)
            {
                Debug.Log($"[{nameof(UnityPurchase)}] Purchase Not initialized.");
                return;
            }

            var product = _storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log($"[{nameof(UnityPurchase)}] Purchase {productId}: PROCESSING.");
                PurchaseProcessing?.Invoke(new PurchaseProcess(product.definition.id));
                _storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log($"[{nameof(UnityPurchase)}] Purchase {productId}: FAILED.");
            }
        }

        public void RestorePurchases()
        {
            if (!IsInitialized)
            {
                Debug.Log($"[{nameof(UnityPurchase)}] Purchase Not initialized.");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                var apple = _storeExtensionProvider.GetExtension<IAppleExtensions>();
                apple.RestoreTransactions((result, message) =>
                {
                    if (message != null)
                    {
                        Debug.Log($"[{nameof(UnityPurchase)}] RestorePurchases Failed. {message}");
                    }
                });
            }
            else
            {
                Debug.Log($"[{nameof(UnityPurchase)}] RestorePurchases FAILED. Not supported on {Application.platform} platform.");
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseArgs)
        {
            var product = purchaseArgs.purchasedProduct;

            Debug.Log($"[{nameof(UnityPurchase)}] Purchase {product.definition.id} Process with receipt {product.receipt}");

            var purchaseState = PurchaseState.Purchased;
            if (_localValidator != null)
            {
                purchaseState = _localValidator.Validate(product.definition.id, product.definition.type.ToString(), product.receipt);
            }

            if (purchaseState == PurchaseState.Purchased && _serverValidator != null)
            {
                _serverValidator.Validate(product.definition.id, product.definition.type.ToString(), product.receipt);
            }

            Debug.Log($"[{nameof(UnityPurchase)}] Purchase {product.definition.id} Process with state {purchaseState}");

            if (purchaseState == PurchaseState.Purchased)
            {
                var purchase = new PurchaseComplete(product.definition.id, product.receipt);
                PurchaseCompleted?.Invoke(purchase);
            }

            return PurchaseProcessingResult.Complete;
        }

        private void ConfirmPurchase(string productId)
        {
            if (_storeController != null)
            {
                var product = _storeController.products.WithID(productId);
                if (product != null && !string.IsNullOrEmpty(product.transactionID))
                {
                    Debug.Log($"[{GetType().Name}] ConfirmPurchase {productId}: Confirm Pending.");
                    _storeController.ConfirmPendingPurchase(product);
                }
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"[{nameof(UnityPurchase)}] OnPurchaseFailed {product.definition.id}: FAILED. PurchaseFailureReason: {failureDescription.message}");
            OnPurchaseFailed(product, failureDescription.reason);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"[{nameof(UnityPurchase)}] OnPurchaseFailed {product.definition.id}: FAILED. PurchaseFailureReason: {failureReason}");
            PurchaseStatus statusType;
            switch (failureReason)
            {
                case PurchaseFailureReason.PurchasingUnavailable:
                    statusType = PurchaseStatus.PurchasingUnavailable;
                    break;
                case PurchaseFailureReason.ExistingPurchasePending:
                    statusType = PurchaseStatus.ExistingPurchasePending;
                    break;
                case PurchaseFailureReason.ProductUnavailable:
                    statusType = PurchaseStatus.ProductUnavailable;
                    break;
                case PurchaseFailureReason.SignatureInvalid:
                    statusType = PurchaseStatus.SignatureInvalid;
                    break;
                case PurchaseFailureReason.UserCancelled:
                    statusType = PurchaseStatus.UserCancelled;
                    break;
                case PurchaseFailureReason.PaymentDeclined:
                    statusType = PurchaseStatus.PaymentDeclined;
                    break;
                case PurchaseFailureReason.DuplicateTransaction:
                    statusType = PurchaseStatus.DuplicateTransaction;
                    break;
                case PurchaseFailureReason.Unknown:
                    statusType = PurchaseStatus.Unknown;
                    break;
                default:
                    statusType = PurchaseStatus.Unknown;
                    break;
            }

            var status = new PurchaseError(product.definition.id, statusType);
            PurchaseFailed?.Invoke(status);
        }
    }
}