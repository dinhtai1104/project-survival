using Assets.Game.Scripts.BaseFramework.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Subscription.Services
{
    public class SubscriptionService : Service
    {
        public SubscriptionTable table;
        public SubscriptionSaves save;

        public override void OnInit()
        {
            base.OnInit();
            table = DataManager.Base.Subscription;
            save = DataManager.Save.Subscription;
        }
        public void Reward()
        {
            save.Reward();
        }

        public void ApplySubscription()
        {
            save.ApplySubscription();
        }


        public void ActiveSubscription(ESubscription sub)
        {
            save.ActiveSubscription(sub);
        }

        public bool CanRewardDaily()
        {
            return save.CanRewardDaily();
        }


        public bool IsFreeRewardAd()
        {
            return save.IsFreeRewardAd();
        }

        public float GetExtraGoldDungeon()
        {
            return save.GetExtraGoldDungeon();
        }
        public float GetExtraExpDungeon()
        {
            return save.GetExtraExpDungeon();
        }
        public List<LootParams> GetAllDailyRewards()
        {
            return save.GetAllDailyRewards();
        }
        public List<LootParams> GetDailyRewards(int Id)
        {
            return save.GetDailyRewards(Id);
        }

        public bool IsActiveAll()
        {
            return save.IsActiveAll();
        }

        public bool IsActiveAny()
        {
            return save.IsActiveAny();
        }
    }
}
