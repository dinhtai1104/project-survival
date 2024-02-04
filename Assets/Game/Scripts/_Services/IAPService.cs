using Assets.Game.Scripts.BaseFramework.Architecture;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;

namespace Assets.Game.Scripts._Services
{
    [System.Serializable]
    public class IAPService : Service
    {
        public override void OnInit()
        {
            base.OnInit();
            IAPManager.Instance.OnPurchaseEvent += OnPurchaseComplete;
        }
        public override void OnDispose()
        {
            base.OnDispose();
            IAPManager.Instance.OnPurchaseEvent -= OnPurchaseComplete;
        }
        private void OnPurchaseComplete(IAPManager.PurchaseState status, IAPPackage package)
        {
            FirebaseAnalysticController.Tracker.NewEvent("purchase_click")
                .AddStringParam("product_id", package.id)
                .Track();
            if (status == IAPManager.PurchaseState.Success)
            {
                FirebaseAnalysticController.Tracker.NewEvent("purchase_success")
                .AddStringParam("product_id", package.id)
                .AddFloatParam("price", package.price)
                .Track();
                DataManager.Save.User.BuyIAP(package.id);
            }
            if (status != IAPManager.PurchaseState.Success)
            {
                PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.PurchaseFailed")).Forget();
            }
        }
    }
}
