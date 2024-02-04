using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSaveCheckPoint : MonoBehaviour
{
    private void OnEnable()
    {
        CloudSave.Controller.Instance.ValidateAndSave().Forget();
        IAPManager.Instance.OnPurchaseComplete += OnPurchased;
    }
    private void OnDisable()
    {
        IAPManager.Instance.OnPurchaseComplete -= OnPurchased;
    }
    private void OnDestroy()
    {
        IAPManager.Instance.OnPurchaseComplete -= OnPurchased;
    }
    private void OnPurchased(IAPManager.PurchaseState arg0, IAPPackage arg1)
    {
        if (arg0 == IAPManager.PurchaseState.Success)
        {
            Invoke(nameof(Save), 1);
        }
    }

    void Save()
    {
        CloudSave.Controller.Instance.ValidateAndSave().Forget();

    }
}
