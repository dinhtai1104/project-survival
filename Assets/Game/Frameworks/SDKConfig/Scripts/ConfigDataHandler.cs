using Cysharp.Threading.Tasks;
using Game.SDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigDataHandler
{
    private const string Path = "SdkConfig";
    public static SDKConfigData configData;

    public delegate void OnUpdate(SDKConfigData newData);
    public static OnUpdate onUpdate;

    public static void ApplyData(string data)
    {
        configData = Newtonsoft.Json.JsonConvert.DeserializeObject<SDKConfigData>(data);
        onUpdate?.Invoke(configData);
    }
    public static async UniTask<SDKConfigData> GetConfig()
    {
        if (configData == null)
        {
            ResourceRequest request= Resources.LoadAsync<SDKConfigData>(Path);
            configData=(await request.ToUniTask()) as SDKConfigData;
        }
        return configData;
    }
}
