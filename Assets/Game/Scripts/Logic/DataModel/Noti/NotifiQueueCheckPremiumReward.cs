using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;

namespace Assets.Game.Scripts.Logic.DataModel.Noti
{
    public class NotifiQueueCheckPremiumReward : NotifiQueueData
    {
        public override bool IsShowOnly => false;
        public NotifiQueueCheckPremiumReward(string type) : base(type)
        {
        }

        public async override UniTask Notify()
        {
            var battlePass = Architecture.Get<BattlePassService>();
            if (!battlePass.IsPremium) return;
            if (battlePass.CanRewardDaily() == false) return;
            var ui = await PanelManager.ShowRewards(new LootParams
            {
                Type = ELootType.Resource,
                Data = new ResourceData(EResource.ReviveCard, 1)
            });
            bool wait = false;
            ui.SetTitle(I2Localize.GetLocalize("Common/Title_BattlePass_DailyReward"));
            ui.onClosed += () => wait = true;
            battlePass.ClaimPremiumDaily();
            await UniTask.WaitUntil(() => wait);
        }
    }
}
