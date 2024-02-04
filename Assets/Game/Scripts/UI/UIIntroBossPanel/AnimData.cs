using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu (menuName  ="AnimationAssetDictionary")]
public class AnimData:ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public string id;
        public AssetReference reference;
        public string title, subTitle;
    }

    private Dictionary<string, Data> dict;
    [SerializeField]
    private Data[] datas;

#if UNITY_EDITOR
    private void OnValidate()
    {
        UnityEditor.EditorUtility.SetDirty(this);
        foreach (var data in datas)
        {
            data.id = data.reference.editorAsset.name;
        }
    }
#endif
 
    public Data Get(string id)
    {
        if (dict == null || dict.Count == 0)
        {
            dict = new Dictionary<string, Data>();

            foreach (var data in datas)
            {
                dict.Add(data.id, data);
            }
        }
        return dict[id];
    }
}