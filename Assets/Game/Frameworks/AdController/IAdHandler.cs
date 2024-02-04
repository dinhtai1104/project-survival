using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface  IAdHandler  
{
    bool IsReady();
    void Init();
    #region banner
    bool IsBannerLoaded();
    void LoadBanner();
    void ShowBanner();
    void HideBanner();
    #endregion

    #region interstitial
    bool IsInterstitialLoaded();
    void LoadInterstitial();
    void ShowInterstitial(System.Action<bool> onShow, System.Action onClose);

    #endregion

    #region reward
    void ShowRewardedAd(string placement,System.Action<bool> onRewared);
    void LoadRewardedAd();
    bool IsRewardAvailable();
  
    #endregion

    #region open ad
    bool IsOpenAdAvailable();
    void LoadOpenAd();
    void ShowOpenAd(System.Action<bool> onShow);


    #endregion
    void LoadNativeAd();
    bool IsNativeAdLoaded();
    object GetCurrentNativeAd();

}


