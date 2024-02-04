using Cysharp.Threading.Tasks;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : UnityEngine.MonoBehaviour
{
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public static FirebaseManager Instance;
    public bool isReady = false;
    public delegate void OnInit();
    public static OnInit onInit;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            Init().Forget();
            DontDestroyOnLoad(gameObject);
        }
    }
   
    bool isInit = false;
    async UniTaskVoid Init()
    {
        if (FirebaseManager.Instance.isInit) return;
        FirebaseManager.Instance.isInit = true;
        try
        {
            UniTask<DependencyStatus> task = FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
            try
            {
                dependencyStatus = await task;
                Logger.Log("FIREBASE INITIALIZED " + dependencyStatus);
                if (dependencyStatus == DependencyStatus.Available)
                {
                    isReady = true;
                    onInit?.Invoke();
                }
                else if (dependencyStatus == DependencyStatus.UnavailableOther)
                {
                    Logger.LogError("UnavailableOther: " + dependencyStatus);
                    isReady = true;
                    onInit?.Invoke();
                }
                else
                {
                    Logger.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            }
            catch (System.Exception e)
            {
                Logger.LogError(e);
            }
        }
        catch(System.Exception e)
        {
            Logger.LogError(e);
            isReady = true;
        }
    }

   
}
