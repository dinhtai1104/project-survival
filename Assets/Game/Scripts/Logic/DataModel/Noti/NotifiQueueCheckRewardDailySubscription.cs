using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Subscription.Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;

namespace Assets.Game.Scripts.Logic.DataModel.Noti
{
    public class NotifiQueueCheckRewardDailySubscription : NotifiQueueData
    {
        public override bool IsShowOnly => false;
        public NotifiQueueCheckRewardDailySubscription(string type) : base(type)
        {
        }

        public async override UniTask Notify()
        {
            var subscription = Architecture.Get<SubscriptionService>();
            if (subscription.CanRewardDaily() == false) return;
            var ui = await PanelManager.ShowRewards(subscription.GetAllDailyRewards());
            bool wait = false;
            ui.SetTitle(I2Localize.GetLocalize("Common/Title_Subscription_DailyReward"));
            ui.onClosed += () => wait = true;
            subscription.Reward();
            await UniTask.WaitUntil(() => wait);
        }
    }
}
