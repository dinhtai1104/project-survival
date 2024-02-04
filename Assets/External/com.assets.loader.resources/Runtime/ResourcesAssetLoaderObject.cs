using com.assets.loader.core;
using UnityEngine;

namespace com.assets.loader.resources
{
    [CreateAssetMenu(fileName = "ResourcesAssetLoader", menuName = "Tata/Assets Loader/Resources Asset Loader")]
    public class ResourcesAssetLoaderObject : AssetLoaderObject, IAssetLoader
    {
        private readonly ResourcesAssetLoader _loader = new ResourcesAssetLoader();

        public override AssetRequest<TAsset> Load<TAsset>(string address)
        {
            return _loader.Load<TAsset>(address);
        }

        public override AssetRequest<TAsset> LoadAsync<TAsset>(string address)
        {
            return _loader.Load<TAsset>(address);
        }

        public override void Release(AssetRequest request)
        {
            _loader.Release(request);
        }
        public override void ReleaseAll()
        {
            _loader.ReleaseAll();
        }
    }
}