using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Subscription.Services;
using Cysharp.Threading.Tasks;
using Firebase.Analytics;
using Game.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts._Services
{
    [System.Serializable]
    public class AdService : Service
    {
        public bool IsFreeRewardAd => subscriptionService.IsFreeRewardAd();
        private SubscriptionService subscriptionService;
        private BattlePassService battlePassService;

        public override void OnInit()
        {
            base.OnInit();
            subscriptionService = Architecture.Get<SubscriptionService>();
            battlePassService = Architecture.Get<BattlePassService>();

            // Register Listener
            AD.Controller.Instance.OnRewardEvent += Instance_OnRewardEvent;
            AD.Controller.Instance.OnLogPlacementAd += Instance_OnLogPlacementAd;
            AdRevenueTracker.sendRevenueEvent += OnTrackRevenue;
        }


        public override void OnDispose()
        {
            base.OnDispose();
            AD.Controller.Instance.OnRewardEvent -= Instance_OnRewardEvent;
            AD.Controller.Instance.OnLogPlacementAd -= Instance_OnLogPlacementAd;
            AdRevenueTracker.sendRevenueEvent -= OnTrackRevenue;
        }

        private void OnTrackRevenue(ImpressionData data)
        {
            SendRevenue(data);
        }
        public void SendRevenue(ImpressionData impressionData)
        {
            FirebaseAnalysticController.Instance.SendRevenue(impressionData);
        }
        private void Instance_OnLogPlacementAd(string placement)
        {
            FirebaseAnalysticController.Tracker.NewEvent(placement).Track();
            FirebaseAnalysticController.Instance.LogFinishRewardAd(placement);
        }

        private void Instance_OnRewardEvent(bool obj)
        {
            if (obj)
            {
                DataManager.Save.User.WatchIAA();
            }
        }
        public void ShowRewardedAd(string place, System.Action<bool> onRewared, string placement = null, bool canSkip = false)
        {
            if (IsFreeRewardAd)
            {
                canSkip = true;
            }
            if (place == AD.AdPlacementKey.BUFF_OFFER)
            {
                if (battlePassService.IsPremium)
                {
                    canSkip = true;
                }
            }

            AD.Controller.Instance.ShowRewardedAd(place, onRewared, placement, canSkip);
        }
    }
}
