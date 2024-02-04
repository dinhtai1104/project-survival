using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Pool
{
    public abstract class SpawnerBase<T> :MonoBehaviour
    {
       
        public abstract  UniTask<T> GetAsync(string id,EPool poolType=EPool.Temporary);
        public abstract  void Get(string id, System.Action<PoolObject> onLoaded, EPool poolType = EPool.Temporary);

        public abstract void ClearAll();
        public abstract void Destroy(bool everything);
        public abstract void ClearPool(EPool poolType,bool useEffect=false);
    }

}