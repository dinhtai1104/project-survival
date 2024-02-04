using UnityEngine;

namespace com.assets.loader.core
{
    public abstract class AssetLoaderObject : ScriptableObject, IAssetLoader
    {
        public abstract AssetRequest<TAsset> Load<TAsset>(string address) where TAsset : Object;

        public abstract AssetRequest<TAsset> LoadAsync<TAsset>(string address) where TAsset : Object;

        public abstract void Release(AssetRequest request);

        public abstract void ReleaseAll();
    }
}