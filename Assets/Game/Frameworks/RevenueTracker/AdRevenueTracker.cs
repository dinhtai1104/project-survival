using System.Collections.Generic;
using UnityEngine;

public class AdRevenueTracker:MonoBehaviour
{
    public static AdRevenueTracker Instance;
    public delegate void SendRevenue(ImpressionData data);
    public static SendRevenue sendRevenueEvent;
    public static void SendRevenueAdmob(GoogleMobileAds.Api.AdValue adValue,string format)
    {
        Instance.AddData(new ImpressionData()
        {
            platform = "admob",
            revenue = adValue.Value / 1000000f,
            currencyCode = adValue.CurrencyCode,
            adFormat = format
        });
    }
    public static void SendRevenueIronsource(IronSourceImpressionData impressionData)
    {
        Instance.AddData(new ImpressionData()
        {
            platform = "ironSource",
            revenue = (double)impressionData.revenue,
            currencyCode = "USD",
            adNetwork = impressionData.adNetwork,
            adUnit = impressionData.instanceName,
            adFormat = impressionData.adUnit,
            placement=impressionData.placement
        });
    }

    public float Delay = 0.25f;

    public Stack<ImpressionData> stack = new Stack<ImpressionData>();
    float time = 0;

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
  
    private void Update()
    {
        if (Time.time - time > Delay)
        {
            time = Time.time;
            if (stack.Count > 0)
            {
                Send(stack.Pop());
            }
        }
    }

    public void AddData(ImpressionData data)
    {
        stack.Push(data);
    }
    public void Send(ImpressionData data)
    {
        sendRevenueEvent?.Invoke(data);
    }
}

