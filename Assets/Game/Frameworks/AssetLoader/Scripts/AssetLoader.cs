using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Asset
{
    public class AssetLoader 
    {
        public static async UniTask<T> LoadAsync<T>(AssetReference reference)
        {
            var result = await Addressables.LoadAssetAsync<T>(reference);
            return result;
        }
        public static void Load<T>(AssetReference reference,System.Action<T> onLoaded)
        {
            Addressables.LoadAssetAsync<T>(reference).Completed+=op=> 
            {
                onLoaded?.Invoke(op.Result);
            };
        }
        public static void Release(object asset)
        {
            if (asset == null) return;
            Addressables.Release(asset);
        }
    }
}