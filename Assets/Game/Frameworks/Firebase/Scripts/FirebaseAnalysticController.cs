using AppsFlyerSDK;
using Cysharp.Threading.Tasks;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;

public class FirebaseAnalysticController : UnityEngine.MonoBehaviour
{
    public static ITrackingProvider Tracker;
    public Dictionary<EResource, double> TrackingResourceEarned = new Dictionary<EResource, double>();

    public static string AD_IMPRESSION = "ad_impression";
    public static string AD_REVENUE_SDK = "ad_revenue_sdk";
    public static string AD_OPEN_NATIVE_IMPRESSION = "cc_openad_native_revenue";


    private const string ADCOUNTEVENT = "ads_reward";
    private const string INAPPPURCHASECOUNTEVENT = "item_purchase";
    private const string AD_LOCATION = "ads_location";
    public static FirebaseAnalysticController Instance;
    StringBuilder builder = new StringBuilder();
    private bool isReady;

    public void  Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            FirebaseManager.onInit -= Init;
            FirebaseManager.onInit += Init;
            if (FirebaseManager.Instance!=null &&FirebaseManager.Instance.isReady)
            {
                Init();
            }
        }
        else
        {
            Destroy(gameObject);
        }
        //AdRevenueTracker.sendRevenueEvent += OnTrackRevenue;

    }

    private void OnDestroy()
    {
       // AdRevenueTracker.sendRevenueEvent -= OnTrackRevenue;
    }

    private void OnTrackRevenue(ImpressionData data)
    {
        SendRevenue(data);
    }

    void Init()
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            isReady = true;
        }
        catch (System.Exception e)
        {
            Logger.LogError(e);
        }
        Tracker = new FirebaseTrackingProvider();

    }

    public void LogEvent(string eventName)
    {
        if (isReady)
        {
            Tracker.NewEvent(eventName).Track();
        }
    }
   
    public void LogFinishRewardAd(string place)
    {
        LogEvent("ADS_REWARD_DONE_"+place);
    }
    public void LogStartRewardAd(string place)
    {
        LogEvent("ADS_REWARD_START_"+place);
    }
    public void LogStartOpenAd()
    {
        LogEvent("open_ad");
    }
    public void LogStartInterstitialAd()
    {
        LogEvent("institial_ad");
    }
    public void LogFinishInterstitialAd()
    {
        LogEvent("institial_ad_finish");
    }


   
    public void LogEvent(string eventName, string parameterKey,string parameterValue)
    {
        if (!isReady) return;
        try
        {
#if UNITY_EDITOR
            builder.Clear();
            builder.Append("Log event:" + eventName);
            builder.AppendLine(parameterKey+" "+parameterValue);
            Logger.Log(builder);
#endif
            Tracker.NewEvent(eventName)
                .AddStringParam(parameterKey, parameterValue)
                .Track();
            //FirebaseAnalytics.LogEvent(eventName, parameterKey,parameterValue);
        }
        catch (System.Exception e)
        {
            Logger.LogError(e);
        }
    }
    List<Parameter> AdParameters = new List<Parameter>();

    public void SendRevenue(ImpressionData impressionData)
    {
        if (!isReady) return;
        AdParameters.Clear();

        if (!string.IsNullOrEmpty(impressionData.platform))
            AdParameters.Add(new Parameter("ad_platform", impressionData.platform));

        if (!string.IsNullOrEmpty(impressionData.adNetwork))
            AdParameters.Add(new Parameter("ad_source", impressionData.adNetwork));

        if (!string.IsNullOrEmpty(impressionData.adUnit))
            AdParameters.Add(new Parameter("ad_unit_name", impressionData.adUnit));

        if (!string.IsNullOrEmpty(impressionData.adFormat))
            AdParameters.Add(new Parameter("ad_format", impressionData.adFormat));

        if (!string.IsNullOrEmpty(impressionData.placement))
            AdParameters.Add(new Parameter("placement", impressionData.placement));

        AdParameters.Add(new Parameter("currency", impressionData.currencyCode));
        AdParameters.Add(new Parameter("value", impressionData.revenue));

        LogEvent(AD_IMPRESSION, AdParameters.ToArray());
        LogEvent(AD_REVENUE_SDK, AdParameters.ToArray());

        switch (impressionData.adFormat)
        {
            case AD.AdKey.OPEN_AD:
            case AD.AdKey.NATIVE_AD:
                LogEvent(AD_OPEN_NATIVE_IMPRESSION, AdParameters.ToArray());
                break;
        }


        Debug.Log($"EVENTREVENUE: {impressionData.ToString()} {AdParameters.Count}");
    }
    public void LogEvent(string eventName, params Parameter[] parameters)
    {
        FirebaseAnalytics.LogEvent(eventName, parameters);

    }

    public void AddTrackingResourceEarn(EResource resource, double value)
    {
        if (TrackingResourceEarned.ContainsKey(resource) == false)
        {
            TrackingResourceEarned.Add(resource, value);
        }
        TrackingResourceEarned[resource] += value;
    }

    public double GetTrackingResourceEarn(EResource resource)
    {
        if (TrackingResourceEarned.ContainsKey(resource) == false) return 0;
        return TrackingResourceEarned[resource];
    }

    public void SetUserProperties()
    {
        Tracker.SetUserProperty("player_id", DataManager.Save.User.Id)
            .SetUserProperty("appsflyer_id", "6474717017")
            .SetUserProperty("online_time", DataManager.Save.General.TimeOnline.ToString())
            .SetUserProperty("max_stage",  $"{(DataManager.Save.Dungeon.BestStage + 1) + (DataManager.Save.Dungeon.CurrentDungeon + 1) * 100}")
            .SetUserProperty("remaining_energy", DataManager.Save.Resources.GetResource(EResource.Energy).ToString())
            .SetUserProperty("iap_count", DataManager.Save.User.IAP_Count.ToString())
            .SetUserProperty("iaa_count", DataManager.Save.User.IAA_Count.ToString())
            .SetUserProperty("remaining_coin", DataManager.Save.Resources.GetResource(EResource.Gold).ToString())
            .SetUserProperty("total_coin_earn", DataManager.Save.Resources.GetResource(EResource.Gold).ToString())
            .SetUserProperty("remaining_gem", DataManager.Save.Resources.GetResource(EResource.Gem).ToString())
            .SetUserProperty("total_gem_earn", DataManager.Save.Resources.GetResource(EResource.Gem).ToString());
    }
}

