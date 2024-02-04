using Assets.Game.Scripts.BaseFramework.Architecture;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : LiveSingleton<DataManager>, IService
{
    public static DatasaveManager Save;
    public static DatabaseManager Base;
    public static DataliveManager Live;

    public void OnInit()
    {
        Logger.Log("Service " + this.GetType() + " On Init", Color.blue);
        DataliveManager.Instance.Init(transform);
        DatabaseManager.Instance.Init(transform);
        DatasaveManager.Instance.Init(transform);
    }

    public async void ReloadDatabase()
    {
        Base.Reload();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        GameSceneManager.Instance.InitializePlayerData();
    }
    public void OnStart()
    {
        Logger.Log("Service " + this.GetType() + " On Start", Color.yellow);
    }
    public void OnDispose()
    {
        Logger.Log("Service " + this.GetType() + " On Dispose", Color.red);
    }
}
