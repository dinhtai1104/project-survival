using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Subscription.Services;
using Cysharp.Threading.Tasks;
using Game.SDK;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AD
{
    public static class AdPlacementKey
    {
        public const string REVIVE = "revive";
        public const string SILVER_CHEST = "silver_chest";
        public const string GOLDEN_CHEST = "golden_chest";
        public const string HERO_CHEST = "hero_chest";
        public const string GOLD_EVENT = "gold_event";
        public const string SCROLL_EVENT = "scroll_event";
        public const string STONE_EVENT = "stone_event";
        public const string PIGGY_BANK = "piggy_bank";
        public const string BUFF_OFFER = "buff_offer";
        public const string TRIAL_HERO = "trial_hero";
        public const string QUEST_AD = "quest_ads";
    }
    public static class AdKey
    {
        public const string OPEN_AD = "app_open";
        public const string NATIVE_AD = "native";
    }
    public class Controller:MonoBehaviour
    {
        private static Controller instance;
        public static Controller Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("AD CONTROLLER");
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<AD.Controller>();
                }
                return instance;
            }

            set
            {
                if (instance == null)
                {
                    instance = value;
                }
            }

        }

        private const string REWARD_COUNT = "RewardCount";
        private const string INTERSTITIAL_COUNT = "InterstitialCount";

        public bool isBusy = false;
        private bool isAd = false;
        private  bool isInit;

        // if ad is active
        public bool IsAd { get => isAd; set => isAd = value; }

        List<IAdHandler> adHandler = new List<IAdHandler>();

        public delegate void OnNativeAdRefresh();
        public OnNativeAdRefresh onNativeAdRefresh;
        public event System.Action<bool> OnRewardEvent;
        public event System.Action<string> OnLogPlacementAd;



        private float lastTimeShowInterstitialAd = 0;
        private float lastTimeShowOpenAd = 0;
        private float lastTimeShowRewardAd = 0;


        // config of the ad
        public AdConfig adConfig;
        public Controller()
        {
            
        }
       
        public void Init(bool isAd)
        {
            if (isInit) return;
            isInit = true;
            Logger.Log("AD INIT");
            this.IsAd = isAd;

            ConfigDataHandler.GetConfig().ContinueWith(OnConfigFetched).Forget();
            ConfigDataHandler.onUpdate += result => adConfig = result.adConfig;



          

            void OnConfigFetched(SDKConfigData config)
            {
                adConfig = config.adConfig;
                //
                GameObject icObj = new GameObject("IronSourceController", typeof(IronSourceAdController));
                IronSourceAdController icAdCtr = icObj.GetComponent<IronSourceAdController>();
                icObj.transform.SetParent(transform);
                adHandler.Add(icAdCtr);
                //ko dung admob thi bo di
                GameObject admobObj = new GameObject("AdmobController", typeof(AdmobController));
                admobObj.transform.SetParent(transform);
                AdmobController admobAdCtr = admobObj.GetComponent<AdmobController>();
                adHandler.Add(admobAdCtr);

                foreach (IAdHandler adHandler in adHandler)
                {
                    adHandler.Init();
                }

                //
                LoadOpenAd();
                LoadBanner();
                LoadInterstitial();
                LoadNativeAd();
            }
        }
        //remove all ad
        public void RemoveAd()
        {
            IsAd = false;
            HideBanner();
            ReloadAllNativeAdBanner();
        }
        // detect if app is focused again, then show open ad
        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                Debug.Log("SHOW OPEN AD PAUSE");
                ShowOpenAd();
            }
        }

        #region reward ad
        public void ShowRewardedAd(string place, System.Action<bool> onRewared,string placement=null, bool canSkip = false)
        {
#if !UNITY_EDITOR
            // k cho spam gọi show QC tren editor
            if (Time.realtimeSinceStartup - lastTimeShowRewardAd < 5) return;
#endif
            try
            {
                if (canSkip || adConfig.GetProperty(EAdConfigProperty.SKIP_AD) == 1)
                {
                    onRewared?.Invoke(true);
                    return;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }

            isBusy = true;
            lastTimeShowRewardAd = Time.realtimeSinceStartup;


            // hiện màn hình Loading AD
            WaitingPanel.Show(()=>
            {
                Show();
            }); 


            async UniTask Show()
            {
                //log event start Reward ở 1 nút nào đấy
                FirebaseAnalysticController.Instance.LogStartRewardAd(place);
                System.GC.Collect();

                //
                foreach (IAdHandler adHandler in adHandler)
                {
                    adHandler.ShowRewardedAd(placement,(async (result) =>
                    {
                        Logger.Log("AD REWARD :" + result);
                        if (result)
                        {
                            PlayerPrefs.SetInt(REWARD_COUNT, PlayerPrefs.GetInt(REWARD_COUNT, 0) + 1);
                            OnLogPlacementAd?.Invoke(place);
                        }
                        await UniTask.Delay(500, ignoreTimeScale: true);
                        // return reward result
                        OnRewardEvent?.Invoke(result);
                        onRewared?.Invoke(result);
                        isBusy = false;

                        WaitingPanel.Hide();


                    }));
                }
            }
           
        }
        #endregion

        #region interstitial ad
        public void ShowInterstitial(System.Action onClosed=null)
        {
            if (!IsAd || adConfig.GetProperty(EAdConfigProperty.SKIP_AD)==1  || Time.time - lastTimeShowInterstitialAd < adConfig.GetProperty(EAdConfigProperty.COOLDOWN))
            {
                onClosed?.Invoke();
                return;
            }
            isBusy = true;

            // hiện màn hình Loading AD
            WaitingPanel.Show(() =>
            {
                Show();
            });


            async UniTask Show()
            {
                await UniTask.Delay(300);
                FirebaseAnalysticController.Instance.LogStartInterstitialAd();

                //// hiện màn hình Loading AD

                foreach (IAdHandler adHandler in adHandler)
                {
                    adHandler.ShowInterstitial(
                        onShow: res =>
                          {
                             if (res)
                             {
                                 PlayerPrefs.SetInt(INTERSTITIAL_COUNT, PlayerPrefs.GetInt(INTERSTITIAL_COUNT, 0) + 1);
                                 FirebaseAnalysticController.Instance.LogFinishInterstitialAd();
                             }
                         },
                        onClose: () =>
                         {
                             Time.timeScale = 1;
                             WaitingPanel.Hide();


                             onClosed?.Invoke();
                            isBusy = false;
                            Debug.Log("INTER FINISH");

                             LoadInterstitial();
                        }
                     );
                }
            }
           
        }
        public void LoadInterstitial()
        {
            if (!IsAd || adConfig.GetProperty(EAdConfigProperty.SKIP_AD)==1 ) return;
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.LoadInterstitial();
            }
        }
        #endregion

        #region banner ad
        public bool IsBannerLoaded()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                if (adHandler.IsBannerLoaded())
                {
                    return true;
                }
            }
            return false;
        }
        public void ShowBanner()
        {
            if (!IsAd || adConfig.GetProperty(EAdConfigProperty.SKIP_AD)==1 ) return;
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.ShowBanner();
            }
        }
        public void HideBanner()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.HideBanner();
            }
        }
        public void LoadBanner()
        {
            if (!IsAd) return;
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.LoadBanner();
            }
        }
        #endregion

        #region open ad
        public void LoadOpenAd()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.LoadOpenAd();
            }
        }
        public void ShowOpenAd()
        {
            Debug.Log("SHOW OPEN AD " + Time.time +" " + lastTimeShowOpenAd);
            if (!IsAd || adConfig.GetProperty(EAdConfigProperty.SKIP_AD)==1 || adConfig.GetProperty(EAdConfigProperty.OPENAD_ENABLED) == 1 || isBusy || !IsOpenAdAvailable()) return;

            // hiện màn hình Loading AD
            WaitingPanel.Show(() =>
            {
                Show();
            },1);


            async UniTask Show()
            {
                await UniTask.Delay(500, ignoreTimeScale: true);
                foreach (IAdHandler adHandler in adHandler)
                {
                    adHandler.ShowOpenAd(async (res) =>
                    {
                        if (res)
                        {
                            lastTimeShowOpenAd = Time.time;
                            FirebaseAnalysticController.Instance.LogStartOpenAd();
                        }
                        await UniTask.Delay(250, ignoreTimeScale: true);
                        WaitingPanel.Hide();
                    });
                }
            }
        }
        public bool IsOpenAdAvailable()
        {
            foreach(IAdHandler adHandler in adHandler)
            {
                if (adHandler.IsOpenAdAvailable())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region native ad
        public void LoadNativeAd()
        {
            Debug.Log("LOAD NATIVE AD");
            foreach(IAdHandler adHandler in adHandler)
            {
                adHandler.LoadNativeAd();
            }
        }
        public bool IsNativeAdAvailable()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                if (adHandler.IsNativeAdLoaded())
                {
                    return true;
                }
            }
            return false;
        }

        public object GetNativeAd()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                if (adHandler.IsNativeAdLoaded())
                {
                    return adHandler.GetCurrentNativeAd();
                }
            }
            LoadNativeAd();
            return null;
        }
       
        public void ReloadAllNativeAdBanner()
        {
            Debug.Log("RELOAD NATIVE AD");
            LoadNativeAd();
            onNativeAdRefresh?.Invoke();
        }

        #endregion
      


    }
}