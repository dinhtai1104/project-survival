using Assets.Game.Scripts._Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;

namespace Assets.Game.Scripts.Logic.DataModel.Noti
{
    public class NotifiQueueCheckShowHeroSale : NotifiQueueData
    {
        public override bool IsShowOnly => false;
        public NotifiQueueCheckShowHeroSale(string type) : base(type)
        {
           
        }

        public async override UniTask Notify()
        {
            var hotSale = DataManager.Save.HotSaleHero;
            var heroSale = hotSale.HeroShowSale;
            if (hotSale.CanShowHeroSale == false) return;

            var ui = await PanelManager.CreateAsync<UIHotSaleHeroDetailPanel>(AddressableName.UIHotSaleHeroDetailPanel);
            ui.Show(heroSale);
            bool wait = false;
            ui.onClosed += () => wait = true;
            await UniTask.WaitUntil(() => wait);

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            hotSale.CanShowHeroSale = false;

            hotSale.HeroShowSale = EHero.None;
            hotSale.Save();
        }
    }
}
