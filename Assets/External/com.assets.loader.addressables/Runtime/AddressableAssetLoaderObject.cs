using com.assets.loader.core;
using UnityEngine;

namespace com.assets.loader.addressables
{
    [CreateAssetMenu(fileName = "AddressableAssetLoader", menuName = "Tata/Assets Loader/Addressable Asset Loader")]
    public class AddressableAssetLoaderObject : AssetLoaderObject, IAssetLoader
    {
        private readonly AddressableAssetLoader _loader = new AddressableAssetLoader();

        public override AssetRequest<TAsset> Load<TAsset>(string address)
        {
            return _loader.Load<TAsset>(address);
        }

        public override AssetRequest<TAsset> LoadAsync<TAsset>(string address)
        {
            return _loader.LoadAsync<TAsset>(address);
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