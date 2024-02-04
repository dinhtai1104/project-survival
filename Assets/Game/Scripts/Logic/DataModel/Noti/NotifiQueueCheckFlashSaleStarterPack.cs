using Assets.Game.Scripts._Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.DataModel.Noti
{
    public class NotifiQueueCheckFlashSaleStarterPack : NotifiQueueData
    {
        public override bool IsShowOnly => false;
        public NotifiQueueCheckFlashSaleStarterPack(string type) : base(type)
        {
        }

        public async override UniTask Notify()
        {
            if (DataManager.Save.FlashSale.ShowStarterPack) return;

            var btn = PanelManager.Instance.GetPanel<UIMainMenuPanel>().GetFeatureButton(EFeature.StarterPack);
            btn.GetComponent<CanvasGroup>().alpha = 1;
            btn.SetActive(true);
                
            var ui = await PanelManager.CreateAsync<UIFlashSaleStarterPackPanel>(AddressableName.UIFlashSaleStarterPackPanel);
            ui.Show();
            var close = false;
            ui.onClosed += () =>
            {
                close = true;
            };
            await UniTask.WaitUntil(() => close);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            DataManager.Save.FlashSale.ShowStarterPack = true;
            DataManager.Save.FlashSale.Save();
        }
    }
}
