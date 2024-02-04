using UnityEngine;

namespace com.assets.loader.core
{
    public interface IAssetLoader
    {
        AssetRequest<TAsset> Load<TAsset>(string address) where TAsset : Object;

        AssetRequest<TAsset> LoadAsync<TAsset>(string address) where TAsset : Object;

        void Release(AssetRequest request);
        void ReleaseAll();
    }
}