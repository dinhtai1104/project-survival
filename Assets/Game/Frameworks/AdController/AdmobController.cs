using Cysharp.Threading.Tasks;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AD
{
    public class NativeAdPack
    {
        public NativeAd nativeAd;
        public bool isReady = false;
    }
    public class AdmobController : MonoBehaviour, IAdHandler
    {

        private string openAdId,nativeAdId;
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private AppOpenAd appOpenAd;
        public Stack<NativeAd> nativeAds = new Stack<NativeAd>();

        public void Init()
        {
            ConfigDataHandler.GetConfig().ContinueWith(OnConfigLoaded).Forget();

            void OnConfigLoaded(Game.SDK.SDKConfigData config)
            {
                if (config.useAdTest_Admob)
                {
                    openAdId = config.sdkIdConfig.GetID(Game.SDK.SDKIdConfig.EIDType.ADMOB_OPEN_AD).GetValue();
                    nativeAdId = config.sdkIdConfig.GetID(Game.SDK.SDKIdConfig.EIDType.ADMOB_NATIVE_AD).GetValue();
                }
                else
                {
                    openAdId = config.sdkIdConfig.GetID(Game.SDK.SDKIdConfig.EIDType.ADMOB_OPEN_AD).GetValue();
                    nativeAdId = config.sdkIdConfig.GetID(Game.SDK.SDKIdConfig.EIDType.ADMOB_NATIVE_AD).GetValue();
                }
#if UNITY_EDITOR
                Logger.Log("OpenadId " + openAdId);
                Logger.Log("nativeAdId " + nativeAdId);
#endif
            }

            GoogleMobileAds.Api.MobileAds.Initialize((Action<InitializationStatus>)((GoogleMobileAds.Api.InitializationStatus initStatus) =>
            {
                // This callback is called once the MobileAds SDK is initialized.
                Logger.Log("INIT ADMOB");
            }));


        }
        System.Action<bool> onShow;
        public void ShowOpenAd(System.Action <bool> onShow)
        {
            this.onShow = onShow;
            if (appOpenAd != null )
            {
                Logger.Log("Showing app open ad.");
                appOpenAd.Show();

            }
            else
            {
                Logger.LogError("App open ad is not ready yet.");
                onShow?.Invoke(false);
                LoadOpenAd();
            }

        }
        public void LoadOpenAd()
        {
            if (appOpenAd != null) return;

            Logger.Log("Loading the app open ad.");

            var adRequest = new AdRequest();
            AppOpenAd.Load(openAdId, ScreenOrientation.Portrait, adRequest,
                (Action<AppOpenAd, LoadAdError>)((AppOpenAd ad, LoadAdError error) =>
                {
              if (error != null || ad == null)
                    {
                        Logger.LogError("app open ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }
                    Logger.Log("App open ad loaded with response : "
                              + ad.GetResponseInfo());
                    appOpenAd = ad;
                    appOpenAd.OnAdPaid += AppOpenAd_OnPaidEvent;
                    appOpenAd.OnAdFullScreenContentClosed += AppOpenAd_OnAdDidPresentFullScreenContent; ;
                }));
        }

       

        private void AppOpenAd_OnAdDidPresentFullScreenContent()
        {
            Logger.Log("ADMOB OnAdFullScreenContentClosed");
            onShow?.Invoke(true);
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            } 

            LoadOpenAd();

        }

        private void AppOpenAd_OnPaidEvent(AdValue adValue)
        {
            Logger.Log("ADMOB ONPAID EVENT");
            //FirebaseAnalysticController.Instance.SendRevenueToFirebase(adValue);
            //Adjust.SendRevenueToAdjust(adValue);

            AdRevenueTracker.SendRevenueAdmob(adValue,AD.AdKey.OPEN_AD);
        }

        public bool IsOpenAdAvailable()
        {
            return appOpenAd != null;
        }

        public bool IsReady()
        {
            return true;
        }

        public bool IsRewardAvailable()
        {
            return false;

        }

        public void LoadBanner()
        {
        }

        public void LoadInterstitial()
        {
        }
        public void LoadRewardedAd()
        {
        }
        public void HideBanner()
        {
        }

        public void ShowBanner()
        {
        }

        public void ShowInterstitial(System.Action<bool> onShow, System.Action onClose)
        {
        }

        

        public void ShowRewardedAd(string placement, Action<bool> onRewared)
        {
        }

        public void ShowRewardedAd2()
        {
        }

        public bool IsBannerLoaded()
        {
            return false;
        }

        #region NativeAd
        public void LoadNativeAd()
        {
            Debug.Log("ADMOB LOAD NATIVE AD");
            AdLoader adLoader = new AdLoader.Builder(nativeAdId)
         .ForNativeAd().SetNumberOfAdsToLoad(1)
         .Build();
            adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
            adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
            adLoader.OnNativeAdImpression += AdLoader_OnNativeAdImpression;
            adLoader.OnNativeAdClicked += AdLoader_OnNativeAdClicked ;
            adLoader.OnNativeAdOpening += AdLoader_OnNativeAdOpening  ; 
            adLoader.LoadAd(new AdRequest());
        }

        private void AdLoader_OnNativeAdOpening(object sender, EventArgs e)
        {
            Debug.Log("Native ad AdLoader_OnNativeAdOpening ");

        }

        private void AdLoader_OnNativeAdClicked(object sender, EventArgs e)
        {
            Debug.Log("Native ad AdLoader_OnNativeAdClicked ");
        }

        private void AdLoader_OnNativeAdImpression(object sender, EventArgs e)
        {
            Debug.Log("Native ad AdLoader_OnNativeAdImpression ");
        }

        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.Log("Native ad HandleAdFailedToLoad ");
            //Invoke(nameof(LoadNativeAd), 5);

        }

        private void HandleNativeAdLoaded(object sender, NativeAdEventArgs e)
        {
            NativeAd nativeAd = e.nativeAd;
            nativeAd.OnPaidEvent -= NativeAd_OnPaidEvent;
            nativeAd.OnPaidEvent += NativeAd_OnPaidEvent;

            nativeAds.Push(nativeAd);
            Debug.Log("Native ad loaded " + e.nativeAd.GetHeadlineText()+" "+ nativeAd.GetResponseInfo().GetResponseId()+" count:"+nativeAds.Count);
        }

        private void NativeAd_OnPaidEvent(object sender, AdValueEventArgs e)
        {

            Debug.Log("ADMOB natve ONPAID EVENT");
            AdRevenueTracker.SendRevenueAdmob(e.AdValue,AD.AdKey.NATIVE_AD);

        }

        public void ShowNativeAd(Action<bool> onShow)
        {
        }

        public bool IsNativeAdLoaded()
        {
            return nativeAds.Count > 0;
        }

        public object GetCurrentNativeAd()
        {
            Debug.Log("GET NATIVE AD: " + nativeAds.Count);
            if (nativeAds.Count > 0)
            {
                Debug.Log("+native>>>>> " + nativeAds.Peek().GetHeadlineText());
                return nativeAds.Pop();
            }
            else
            {
                return null;
            }
        }

        public bool IsInterstitialLoaded()
        {
            return false;
        }

        #endregion
    }
}