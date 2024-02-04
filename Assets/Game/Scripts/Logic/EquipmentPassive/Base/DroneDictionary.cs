using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu( menuName ="Drone/Dictionary")]
public class DroneDictionary : ScriptableObject
{
    [SerializeField]
    private Config[] configs;

    private Dictionary<string, AssetReference> dictionary;

    public string Get(string id)
    {
        if (dictionary == null)
        {
            dictionary = new Dictionary<string, AssetReference>();

            foreach(var config in configs)
            {
                dictionary.Add(config.key, config.obj);
            }
        }
        return dictionary[id].RuntimeKey.ToString();
    }

    [System.Serializable]
    public struct Config 
    {
        public string key;
        public AssetReference obj;
    }

}