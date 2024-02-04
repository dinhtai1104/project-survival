using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;
using System;

// This class is intended to be used the the AppsFlyerObject.prefab

public class AppsFlyerHandler : MonoBehaviour, IAppsFlyerConversionData
{
    private const string AD_REVENUE = "af_ad_revenue";
    private const string PURCHASE = "af_purchase";

    // These fields are set from the editor so do not modify!
    //******************************//
    public string devKey;
    public string appID;
    public string UWPAppID;
    public string macOSAppID;
    public bool isDebug;
    public bool getConversionData;
    //******************************//


    void Start()
    {
        Logger.Log("START APPSFLYER " + devKey + " " + appID);

        DontDestroyOnLoad(gameObject);
        // These fields are set from the editor so do not modify!
        //******************************//
        Logger.Log("APPSFLYER SETdebug " + isDebug);
        AppsFlyer.setIsDebug(isDebug);
#if UNITY_WSA_10_0 && !UNITY_EDITOR
        AppsFlyer.initSDK(devKey, UWPAppID, getConversionData ? this : null);
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
    AppsFlyer.initSDK(devKey, macOSAppID, getConversionData ? this : null);
#else
        Logger.Log("APPSFLYER initSDK " + devKey+" "+ appID);
        AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);
        Logger.Log("APPSFLYER OnInAppResponse ");
        AppsFlyer.OnInAppResponse += AppsFlyer_OnInAppResponse;
        Logger.Log("APPSFLYER OnRequestResponse ");
        AppsFlyer.OnRequestResponse += AppsFlyer_OnRequestResponse ;


#endif



        //******************************/

        Logger.Log("APPSFLYER START SDK1");
        AppsFlyer.startSDK();
        Logger.Log("APPSFLYER START SDK2");

        if (PlayerPrefs.GetInt("FirstOpen",0)==0)
        {
            PlayerPrefs.SetInt("FirstOpen", 1);
            AppsFlyer.sendEvent("af_first_open",null);
        }

#if UNITY_IOS && !UNITY_EDITOR
        AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
#endif
    }

    private void AppsFlyer_OnRequestResponse(object sender, System.EventArgs e)
    {
        Logger.Log("AppsFlyer_OnRequestResponse "+e.ToString());
    }

    private void AppsFlyer_OnInAppResponse(object sender, System.EventArgs e)
    {
        Logger.Log("AppsFlyer_OnInAppResponse " + e.ToString());
    }

    void OnEnable()
    {
        AdRevenueTracker.sendRevenueEvent += OnTrackRevenue;
        IAPManager.Instance.OnPurchaseEvent += OnPurchaseComplete;
    }
    void OnDestroy()
    {
        AdRevenueTracker.sendRevenueEvent -= OnTrackRevenue;

        IAPManager.Instance.OnPurchaseEvent -= OnPurchaseComplete;
    }

    private void OnPurchaseComplete(IAPManager.PurchaseState result, IAPPackage package)
    {
        if (result == IAPManager.PurchaseState.Success)
        {
            parameters.Clear();
            parameters.Add("product_id", package.id);
            parameters.Add("af_revenue", package.price.ToString());

            AppsFlyer.sendEvent(PURCHASE, parameters);

            Debug.Log($"EVENTREVENUE APPSFLYER: {package.id.ToString()} {parameters.Count}");
        }
    }

    private void OnTrackRevenue(ImpressionData data)
    {

        SendRevenue(data);
    }
    Dictionary<string,string> parameters = new Dictionary<string, string>();

    public void SendRevenue(ImpressionData impressionData)
    {

        //if (!isReady) return;
        parameters.Clear();

        if (!string.IsNullOrEmpty(impressionData.platform))
            parameters.Add("ads_network", impressionData.platform);
        if (!string.IsNullOrEmpty(impressionData.placement))
            parameters.Add("ad_location", impressionData.placement);

        if (!string.IsNullOrEmpty(impressionData.adFormat))
            parameters.Add("ads_type", impressionData.adFormat);

        //parameters.Add("currency", impressionData.currencyCode);
        parameters.Add("af_revenue", impressionData.revenue.ToString());

        AppsFlyer.sendEvent(AD_REVENUE, parameters);

        Debug.Log($"EVENTREVENUE APPSFLYER: {impressionData.ToString()} {parameters.Count}");
    }
    // Mark AppsFlyer CallBacks
    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("didReceiveConversionData", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        // add deferred deeplink logic here
    }

    public void onConversionDataFail(string error)
    { 
        AppsFlyer.AFLog("didReceiveConversionDataWithError:", error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }

}
