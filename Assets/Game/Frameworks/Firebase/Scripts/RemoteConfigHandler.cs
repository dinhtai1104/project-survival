using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Firebase.RemoteConfig;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.AddressableAssets;

public class RemoteConfigHandler : UnityEngine.MonoBehaviour
{
    public static RemoteConfigHandler Instance;
    public bool isReady = false;
    private Dictionary<string, string> configDictionary = new Dictionary<string, string>();
    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            FirebaseManager.onInit -= GetDataAndActive;
            FirebaseManager.onInit += GetDataAndActive;
            if (FirebaseManager.Instance!=null&& FirebaseManager.Instance.isReady)
            {
                try
                {
                    GetDataAndActive();
                }
                catch (System.Exception e)
                {
                    Logger.LogError(e);
                    isReady = true;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void GetDataAndActive()
    {
        //set giá trị default cho remote
        try
        {
            //UniTask setDefaultTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).AsUniTask();
            //// đợi khi nào task set giá trị default thành công
            //await setDefaultTask;
            // đợi firebase lấy dữ liệu remote xong
            await Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).AsUniTask();
        }
        catch (System.Exception e)
        {
            Logger.LogError(e);
        }
        //lấy dữ liệu từ remote
        FetchComplete().Forget();
    }

    private async UniTaskVoid FetchComplete()
    {
        await UniTask.WaitUntil(() => FirebaseManager.Instance.isReady);

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        configDictionary = new Dictionary<string, string>();
        string[] configKeys = { "SdkConfig", "LatestVersion" };
        //fetching configs from server
        try
        {
            UniTask<bool> activeTask = FirebaseRemoteConfig.DefaultInstance.ActivateAsync().AsUniTask();
            await activeTask;
            //get data
            foreach (string key in configKeys)
            {
                configDictionary.Add(key, FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue);
            }
        }
        catch (System.Exception e)
        {
            Logger.LogError(e);
        }

        foreach(var data in configDictionary)
        {
            Debug.Log(data.Key + " => " + data.Value);
        }
        //process final data here
        if (!string.IsNullOrEmpty(configDictionary["SdkConfig"]))
        {
            ConfigDataHandler.ApplyData(configDictionary["SdkConfig"]);
        }
     
        //done,remoteconfig is ready
        FirebaseManager.onInit -= GetDataAndActive;
        isReady = true;
    }

    public string GetValue(string key)
    {
        return configDictionary[key];
    }

    private void OnDestroy()
    {
        FirebaseManager.onInit -= GetDataAndActive;
    }

}
