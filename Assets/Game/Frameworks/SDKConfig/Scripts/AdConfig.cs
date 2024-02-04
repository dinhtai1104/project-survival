using System.Collections.Generic;
using UnityEngine;

namespace Game.SDK
{
    [System.Serializable]
    public class AdConfig
    {
        public AdProperty[] configs;

        private Dictionary<EAdConfigProperty, AdProperty> configDict;

        public int GetProperty(EAdConfigProperty type)
        {
            if (configDict == null)
            {
                configDict = new Dictionary<EAdConfigProperty, AdProperty>();

                foreach(AdProperty adProperty in configs)
                {
                    configDict.Add(adProperty.key, adProperty);
                }
            }
            if (configDict.ContainsKey(type))
            {
                return configDict[type].value;
            }
            return default;
        }
        public AdProperty Get(EAdConfigProperty type)
        {
            if (configDict == null)
            {
                configDict = new Dictionary<EAdConfigProperty, AdProperty>();

                foreach (AdProperty adProperty in configs)
                {
                    configDict.Add(adProperty.key, adProperty);
                }
            }
            return configDict[type];
        }
    }
    [System.Serializable]
    public class AdProperty
    {
        public EAdConfigProperty key;
        public int value;

        public AdProperty()
        {
        }
    }
    public enum EAdConfigProperty
    {
        STARTAFTER,COOLDOWN,GAMECOUNT,OPENAD_ENABLED,SKIP_AD
    }
}