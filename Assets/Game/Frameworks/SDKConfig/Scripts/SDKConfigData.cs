using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SDK
{
    [CreateAssetMenu(menuName ="SDK/Config")]
    public class SDKConfigData : ScriptableObject
    {
        public SDKIdConfig sdkIdConfig;
        public AdConfig adConfig;
        public bool useAdTest_Admob;
        [Header("Ironsource")]
        public bool adapterDebug, intergrationHelper,debugger;
       
    }

    [System.Serializable]
    public class SDKIdConfig
    {

        [Header("ID LIST")]
        public ID[] ids;
        //
     

        private Dictionary<EIDType, ID> idDict;

        public SDKIdConfig()
        {
        }

        public ID GetID(EIDType key)
        {
            if (idDict == null)
            {
                idDict = new Dictionary<EIDType, ID>();
                foreach(var id in ids)
                {
                    idDict.Add(id.key, id);
                }
            }

            Logger.Log("GET ID : " + key + " " + idDict.ContainsKey(key));
            if (idDict.ContainsKey(key))
            {
                return idDict[key];
            }
            else
            {
                return new ID();
            }
        }



        [System.Serializable]
        public class ID
        {
            public EIDType key;
            public ObscuredString[] values;

            public ID()
            {
            }

            public ObscuredString GetValue()
            {
                if (values == null || values.Length == 0) return string.Empty;
#if UNITY_ANDROID
                return values[(int)EPlatform.Android];
#elif UNITY_IOS
                return values[(int)EPlatform.IOS];
#endif
            }
        }
        public enum EIDType
        {
            ADMOB_APP_KEY,
            ADMOB_INTERSTITIAL_AD,
            ADMOB_REWARD_AD,
            ADMOB_BANNER_AD,
            ADMOB_OPEN_AD,
            ADMOB_NATIVE_AD,
            
            IRONSOURCE_APP_KEY,
            IRONSOURCE_INTERSTITIAL_AD,
            IRONSOURCE_REWARD_AD,
            IRONSOURCE_BANNER_AD,
            IRONSOURCE_OPEN_AD,
            IRONSOURCE_NATIVE_AD,
            ADJUST,

        
        }
        public enum EPlatform
        {
            Android, IOS,Test
        }
    }
}