using System;
using UnityEngine;

public class StarterPackUnlockable : MonoBehaviour, IUnlockable
{
    private void Start()
    {
        IAPManager.Instance.OnPurchaseEvent += OnPurchase;
    }

    private void OnDestroy()
    {
        IAPManager.Instance.OnPurchaseEvent -= OnPurchase;
    }

    private void OnPurchase(IAPManager.PurchaseState state, IAPPackage package)
    {
        if (state != IAPManager.PurchaseState.Success) return;
        if (package.id == DataManager.Base.FlashSale.Get(EFlashSale.StarterPack).ProductId)
        {
            GetComponent<UIButtonFeature>().InitFirst();
        }
    }

    public bool Validate()
    {
        if (!DataManager.Save.FlashSale.ShowStarterPack)
        {
            return false;
        }
        var save = DataManager.Save.FlashSale.GetSave(EFlashSale.StarterPack);
        if (save != null)
        {
            if (save.Count < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}