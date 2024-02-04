using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AD
{
    public class IronSourceAdController : MonoBehaviour, IAdHandler
    {
        bool isReady = false;
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        public void Init()
        {
            ConfigDataHandler.GetConfig().ContinueWith(OnConfigLoaded).Forget();

            void OnConfigLoaded(Game.SDK.SDKConfigData config)
            {
                string appKey = config.sdkIdConfig.GetID(Game.SDK.SDKIdConfig.EIDType.IRONSOURCE_APP_KEY).GetValue();

                Logger.Log("IronSourceInitilizer " + appKey);
                IronSourceConfig.Instance.setClientSideCallbacks(true);

                if (appKey.Equals(string.Empty))
                {
                    Logger.LogWarning("IronSourceInitilizer Cannot init without AppKey");
                }
                else
                {
                    //appKey= PlayerPrefs.GetString("IronSourceAdID", "165cebd45"); 
                    Logger.Log("-IronSourceInitilizer " + appKey);
                    IronSource.Agent.init(appKey);
                    IronSource.UNITY_PLUGIN_VERSION = "7.2.1-ri";
                }

                Logger.Log("Adapter debug " + config.adapterDebug);
                if (config.adapterDebug)
                {
                    Logger.Log("Ironsource EnableAdapterDebug ");
                    IronSource.Agent.setAdaptersDebug(true);
                }

                Logger.Log("EnableIntegrationHelper " + config.intergrationHelper);
                if (config.intergrationHelper)
                {
                    Logger.Log("Ironsource validateIntegration ");
                    IronSource.Agent.validateIntegration();
                }

                //register ironsource event

                IronSourceEvents.onSdkInitializationCompletedEvent -= SdkInitializationCompletedEvent;
                IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
                IronSourceEvents.onImpressionDataReadyEvent -= ImpressionDataReadyEvent;
                IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;


                // init ironsource ad quality
                ISAdQualityConfig iSAdQualityConfig = new ISAdQualityConfig();
                IronSourceAdQuality.Initialize(appKey, iSAdQualityConfig);
#if UNITY_ANDROID|| UNITY_IOS
                IronSource.Agent.setConsent(true);
                IronSource.Agent.setMetaData("do_not_sell", "false");
                IronSource.Agent.setMetaData("is_child_directed", "false");


#endif
            }

#if UNITY_IOS
            try
            {
                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
            }catch(System.Exception e){
            Logger.LogError(e);
            }
#endif


        }
        public bool IsReady()
        {
            return isReady;
        }
        void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            Logger.Log("----------IMPRESSION DATA READY EVENT " + impressionData.adNetwork+" "+impressionData.placement);
            AdRevenueTracker.SendRevenueIronsource(impressionData);
        }
      




        private void SdkInitializationCompletedEvent()
        {
            Logger.Log("Ironsource INITialzied");
            isReady = true;
            //reward
            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdLoadFailedEvent += IronSourceEvents_onRewardedVideoAdLoadFailedEvent;

          
            //banner


            IronSourceBannerEvents.onAdLoadedEvent += IronSourceEvents_onBannerAdLoadedEvent; ;
            IronSourceBannerEvents.onAdLoadFailedEvent += IronSourceEvents_onBannerAdLoadFailedEvent; ;
            IronSourceBannerEvents.onAdClickedEvent += IronSourceEvents_onBannerAdClickedEvent; ;
            IronSourceBannerEvents.onAdScreenPresentedEvent += IronSourceEvents_onBannerAdScreenPresentedEvent; ;
            IronSourceBannerEvents.onAdScreenDismissedEvent += IronSourceEvents_onBannerAdScreenDismissedEvent; ;
            IronSourceBannerEvents.onAdLeftApplicationEvent += IronSourceEvents_onBannerAdLeftApplicationEvent; ;

            // interstitial
            IronSourceInterstitialEvents.onAdReadyEvent += IronSourceEvents_onInterstitialAdReadyEvent; ;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += IronSourceEvents_onInterstitialAdLoadFailedEvent; ;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += IronSourceEvents_onInterstitialAdShowSucceededEvent; ;
            IronSourceInterstitialEvents.onAdShowFailedEvent += IronSourceEvents_onInterstitialAdShowFailedEvent; ;
            IronSourceInterstitialEvents.onAdClickedEvent += IronSourceEvents_onInterstitialAdClickedEvent; ;
            IronSourceInterstitialEvents.onAdOpenedEvent += IronSourceEvents_onInterstitialAdOpenedEvent; ;
            IronSourceInterstitialEvents.onAdClosedEvent += IronSourceEvents_onInterstitialAdClosedEvent; ;

         

        }
#region interstitial event
        private void IronSourceEvents_onInterstitialAdClosedEvent(IronSourceAdInfo adInfo)
        {
            onInstitialClose?.Invoke();
            onInstitialClose = null;
            onInstitialShow = null;
#if UNITY_IOS
            AudioListener.pause = false;
#endif
        }

        private void IronSourceEvents_onInterstitialAdOpenedEvent(IronSourceAdInfo adInfo)
        {

        }

        private void IronSourceEvents_onInterstitialAdClickedEvent(IronSourceAdInfo adInfo)
        {
        }

        private void IronSourceEvents_onInterstitialAdShowFailedEvent(IronSourceError obj, IronSourceAdInfo adInfo)
        {
            OnInterstitialShowFailed();

            IronSource.Agent.SetPauseGame(false);
#if UNITY_IOS
                AudioListener.pause = false;
#endif
            Logger.Log("IronSourceEvents_onInterstitialAdShowFailedEvent " + obj.getDescription() + " " + obj.getErrorCode() + " " + obj.getCode());
        }

        private void IronSourceEvents_onInterstitialAdShowSucceededEvent(IronSourceAdInfo adInfo)
        {
            onInstitialShow?.Invoke(true);
            onInstitialShow = null;
            IronSource.Agent.SetPauseGame(false);
#if UNITY_IOS
                AudioListener.pause = false;
#endif
            Logger.Log("IronSourceEvents_onInterstitialAdShowSucceededEvent ");

        }

        private void IronSourceEvents_onInterstitialAdLoadFailedEvent(IronSourceError obj)
        {
            Logger.Log("IronSourceEvents_onInterstitialAdLoadFailedEvent " + obj.getDescription() + " " + obj.getErrorCode() + " " + obj.getCode());
            OnInterstitialShowFailed();
        }

        private void IronSourceEvents_onInterstitialAdReadyEvent(IronSourceAdInfo adInfo)
        {
            Logger.Log("IronSourceEvents_onInterstitialAdReadyEvent ");
        }
#endregion
#region banner event
        private void IronSourceEvents_onBannerAdLeftApplicationEvent(IronSourceAdInfo adInfo)
        {
        }

        private void IronSourceEvents_onBannerAdScreenDismissedEvent(IronSourceAdInfo adInfo)
        {
        }

        private void IronSourceEvents_onBannerAdScreenPresentedEvent(IronSourceAdInfo adInfo)
        {
        }

        private void IronSourceEvents_onBannerAdClickedEvent(IronSourceAdInfo adInfo)
        {
        }

        private void IronSourceEvents_onBannerAdLoadFailedEvent(IronSourceError obj)
        {
            Logger.Log("IronSource: IronSourceEvents_onBannerAdLoadedEvent" + obj.getErrorCode() + " " + obj.getCode() + " " + obj.getDescription());
            isBannerLoaded = false;
        }

        private void IronSourceEvents_onBannerAdLoadedEvent(IronSourceAdInfo adInfo)
        {
            Logger.Log("IronSource: IronSourceEvents_onBannerAdLoadedEvent");
            isBannerLoaded = true;


        }
#endregion
#region reward event
        private void IronSourceEvents_onRewardedVideoAdLoadFailedEvent(IronSourceError obj)
        {
            Logger.LogError("Ironsource: RewardedVideoAdLoadFailedEvent " + obj.getErrorCode() + " " + obj.getCode() + " " + obj.getDescription());
            OnShowRewardFailed();
            isFinished = true;
        }
        private void RewardedVideoAdShowFailedEvent(IronSourceError obj, IronSourceAdInfo adInfo)
        {
            Logger.LogError("Ironsource: RewardedVideoAdShowFailedEvent " + obj.getErrorCode() + " " + obj.getCode() + " " + obj.getDescription());
            OnShowRewardFailed();
            isFinished = true;

        }

        private void RewardedVideoAdRewardedEvent(IronSourcePlacement obj, IronSourceAdInfo adInfo)
        {
            Logger.Log("Ironsource: RewardedVideoAdRewardedEvent ");
            isRewarded = true;
            isFinished = true;

        }


        private void RewardedVideoAvailabilityChangedEvent(IronSourceAdInfo adInfo)
        {
            Logger.Log("Ironsource: RewardedVideoAvailabilityChangedEvent " + (adInfo!=null));
        }

        private void RewardedVideoAdClosedEvent(IronSourceAdInfo adInfo)
        {
            Logger.Log("Ironsource VIDEO CLOSED "+ isRewarded);
            WaitForResult().Forget();

            async UniTask WaitForResult()
            {
                Logger.Log("Ironsource WaitForResult " + isRewarded);
                float time = Time.time;
                await UniTask.WaitUntil(() => isRewarded ||onRewarded==null || Time.time-time>1);
                Logger.Log("Ironsource WaitForResult FINISH " + isRewarded);
                isAdPlaying = false;
                AudioListener.pause = false;
                Time.timeScale = 1;
                IronSource.Agent.SetPauseGame(false);

                if (isRewarded)
                {
                    OnReward();
                    isRewarded = false;
                }
            }
          
        }

        private void RewardedVideoAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            Logger.Log("Ironsource VIDEO OPENED");
            IronSource.Agent.SetPauseGame(true);
            isAdPlaying = true;
            AudioListener.pause = true;
            Time.timeScale = 0;
        }
        #endregion





        #region rewarded ad
        bool isFinished = false;
        public bool isAdPlaying = false;
        //true if reward success
        bool isRewarded = false;
        Action<bool> onRewarded;
    
        public void ShowRewardedAd(string placement,Action<bool> onRewarded)
        {
            if (onRewarded == null) return;
            isRewarded = false;
            this.onRewarded = onRewarded;

            Logger.Log("Ironsource: showrewardad " + IsRewardAvailable() +" "+(string.IsNullOrEmpty(placement)?"default":placement));
            //if ad is available, then show ad
            if (IsRewardAvailable())
            {
                ShowAd();
            }
            else
            {
                LoadRewardedAd();
                Logger.Log("Waiting for ad");
                WaitForRewardAd(result =>
                {
                    //ad available
                    if (result)
                    {
                        ShowAd();
                    }
                    // ad still not load, call reward fail event
                    else
                    {
                        OnShowRewardFailed();
                    }

                }).Forget();

            }


            void ShowAd()
            {
                if (IsRewardAvailable())
                {
                    Logger.Log("SHOW REWARD: " + (string.IsNullOrEmpty(placement) ? "default" : placement)+ " "+(!string.IsNullOrEmpty(placement)));
                    if (!string.IsNullOrEmpty(placement))
                    {
                        IronSource.Agent.showRewardedVideo(placementName: placement);

                        //try
                        //{
                        //    IronSourcePlacement current = IronSource.Agent.getPlacementInfo(placement);
                        //    if (current == null)
                        //    {
                        //        Logger.Log("NO PLACEMENT NAME: " + placement);
                        //    }
                        //    else
                        //    {
                        //        Logger.Log("PLACEMENT: " + placement + " " + current.getPlacementName());
                        //    }
                        //}
                        //catch { }
                    }
                    else
                    {
                        IronSource.Agent.showRewardedVideo();

                    }

                }
            }
        }

       
        //đợi ad khi nào available thì trả về true, timeout thì trả false
        async UniTaskVoid WaitForRewardAd(System.Action<bool> onAction)
        {
            float timeOut = 8;
            while (timeOut > 0)
            {
                timeOut -= Time.fixedUnscaledDeltaTime;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

                if (IsRewardAvailable())
                {
                    onAction?.Invoke(true);
                    return;
                }
            }
            onAction?.Invoke(false);

        }
        public void LoadRewardedAd()
        {
            Logger.Log("IronSource Loadreward ad");
            IronSource.Agent.loadRewardedVideo();

        }

        public bool IsRewardAvailable()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }
        void OnReward()
        {
            Logger.Log("Ironsource onRewarded " + (onRewarded!=null));
            onRewarded?.Invoke(true);
            onRewarded = null;
        }
        void OnShowRewardFailed()
        {
            onRewarded?.Invoke(false);
            onRewarded = null;
        }
#endregion

#region interstitialAd
        Action<bool> onInstitialShow;
        Action onInstitialClose;
        public bool IsInterstitialLoaded()
        {
            return IronSource.Agent.isInterstitialReady();
        }
        public void LoadInterstitial()
        {
            IronSource.Agent.loadInterstitial();
        }
    
        public void ShowInterstitial(Action<bool> onShow, Action onInstitialClose)
        {
            Logger.Log("IS: show inte");
            this.onInstitialShow = onShow;
            this.onInstitialClose = onInstitialClose;

            if (IsInterstitialLoaded())
            {
#if UNITY_IOS
                AudioListener.pause = true;
#endif
                IronSource.Agent.SetPauseGame(true);

             
                IronSource.Agent.showInterstitial();

            }
            else
            {
                OnInterstitialShowFailed();
                LoadInterstitial();
                Logger.Log("unity-script: IronSource.Agent.isInterstitialReady - False");
            }
        }

        void OnInterstitialShowFailed()
        {
            onInstitialClose?.Invoke();
            onInstitialClose = null;
            onInstitialShow?.Invoke(false);
            onInstitialShow = null;
        }
        void OnInterstitialShowSuccess()
        {

        }
#endregion
#region banner ad
        bool isBannerLoaded = false;
        public bool IsBannerLoaded()
        {
            return isBannerLoaded;
        }
        public void ShowBanner()
        {
            Logger.Log("IronSource: ShowBanner");
            if(IsBannerLoaded())
                IronSource.Agent.displayBanner();
        }
        public void LoadBanner()
        {
            Logger.Log("IronSource: Loadbanner");
            IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
        }

        public void HideBanner()
        {
            Logger.Log("IronSource: HideBanner");
            IronSource.Agent.hideBanner();
        }

#endregion





#region openAD

        public bool IsOpenAdAvailable()
        {
            return false;
        }

        public void LoadOpenAd()
        {
        }

        public void ShowOpenAd(System.Action<bool> onShow)
        {
        }

#endregion

        void OnApplicationPause(bool isPaused)
        {
            Logger.Log("IronSource Pause:" + isPaused);
            IronSource.Agent.onApplicationPause(isPaused);
        }

        public void LoadNativeAd()
        {
        }

        public bool IsNativeAdLoaded()
        {
            return false;
        }

        public object GetCurrentNativeAd()
        {
            return null;
        }
    }
}