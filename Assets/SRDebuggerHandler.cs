using Cysharp.Threading.Tasks;
using Game.SDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRDebuggerHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }
    public void Init()
    {
        ConfigDataHandler.GetConfig().ContinueWith(OnConfigFetched).Forget();
        ConfigDataHandler.onUpdate += OnConfigFetched;





        void OnConfigFetched(SDKConfigData config)
        {
            if (config.debugger)
            {
                SRDebug.Init();
            }
        }
    }
}
