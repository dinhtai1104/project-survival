using Assets.Game.Scripts.BaseFramework.Architecture;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;

namespace Assets.Game.Scripts.BattlePass.UI
{
    public class UIGettingsBattlePassPanel : Panel
    {
        public UILootCollectionView lootView;
        public UIInventorySlot inventorySlotPrefab;
        public TextMeshProUGUI priceTxt;

        public override void PostInit()
        {
        }
        public override void Show()
        {
            base.Show();
            CreateView();
        }

        private void CreateView()
        {
            var db = Architecture.Get<BattlePassService>().Table;
            var allLoot = db.GetAllLoot(EBattlePass.Premium);
            allLoot = allLoot.Merge();
            allLoot = allLoot.OrderBy(t => t.Type).ToList();
            lootView.Show(new LootCollectionData(allLoot), inventorySlotPrefab, OnSpawnInventory)
                .Forget();

            priceTxt.text = $"{IAPManager.Instance.GetPrice(BattlePassTable.PremiumProductId)} {IAPManager.Instance.GetIsoCurrencyCode(BattlePassTable.PremiumProductId)}";
        }

        private void OnSpawnInventory(UIInventorySlot slot)
        {
            slot.transform.localScale = Vector3.one * 0.8f;
        }

        public void OnBuyPremiumClicked()
        {
            IAPManager.Instance.BuyProduct(BattlePassTable.PremiumProductId, OnPurchaseComplete);
        }

        private async void OnPurchaseComplete(IAPManager.PurchaseState status, IAPPackage package)
        {
            if (status == IAPManager.PurchaseState.Success)
            {
                if (package.id == BattlePassTable.PremiumProductId)
                {
                    Architecture.Get<BattlePassService>().BuyPremium();
                    Close();

                    var ui = await PanelManager.ShowRewards(new LootParams
                    {
                        Type = ELootType.Resource,
                        Data = new ResourceData(EResource.ReviveCard, 1)
                    });
                    ui.Show();

                    Architecture.Get<BattlePassService>().ClaimPremiumDaily();
                }
            }
        }
    }
}
