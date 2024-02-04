using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Pool
{
    [System.Serializable]
    public class ObjectCollection
    {
        private const string DisableEffect = "DisableEffect";
        public List<PoolObject> pool = new List<PoolObject>();
        public List<PoolObject> inUsePool = new List<PoolObject>();
        public AsyncOperationHandle<Object> loadRequest;
        bool isReady = false;
        bool isLoading = false;
        public ObjectCollection(AsyncOperationHandle<Object> request)
        {
            loadRequest = request;
        }
       
        public async UniTask Load()
        {
            if (isLoading)
            {
                await UniTask.WaitUntil(() => !isLoading);
                return;
            }
            isLoading = true;
            //try
            //{
                await loadRequest;
            //}
            //catch(System.Exception e)
            //{
            //    Debug.LogError(e);
            //}
            
            isReady = true;
            isLoading = false;
        }

        public void Add(PoolObject obj)
        {
            pool.Add(obj);
            obj.onReleased = Remove;
            obj.onReEnabled = Enable;
        }
        public void Enable(PoolObject obj)
        {
            if (pool.Contains(obj))
            {
                pool.Remove(obj);
                inUsePool.Add(obj);
            }
        }
        public void Remove(PoolObject obj)
        {
            pool.Add(obj);
            inUsePool.Remove(obj);
        }

        // get an object from inuse pool
        public PoolObject Get(Transform parent)
        {
            if (loadRequest.Result == null) return null;
            //if pool is empty, create new object
            if (pool.Count == 0)
            {
                GameObject obj = ((GameObject)GameObject.Instantiate(loadRequest.Result));
                obj.name = loadRequest.Result.name + "_" + (pool.Count + inUsePool.Count);
                obj.transform.SetParent(parent);
                Add(obj.AddComponent<PoolObject>());
            }
            PoolObject readyObj = pool[0];

            //if first obj is null, remove it
            if (readyObj == null)
            {
                pool.RemoveAt(0);
                return Get(parent);
            }
            readyObj.IsAvailable = false;


            //remove from pool and add to inuse pool
            pool.Remove(readyObj);
            inUsePool.Add(readyObj);


            return readyObj;
        }

        //deactive all inuse objects
        public void ClearAll(bool useEffect=false)
        {
            while (inUsePool.Count > 0)
            {
                PoolObject poolObject = inUsePool[0];
                Vector2 position = poolObject.transform.position;

                poolObject.gameObject.SetActive(false);
                inUsePool.Remove(poolObject);

                if (useEffect)
                {
                    Game.Pool.GameObjectSpawner.Instance.Get(DisableEffect, obj =>
                    {
                        obj.GetComponent<Game.Effect.EffectAbstract>().Active(position);
                    });
                }
            }
           
        }

        //destroy everything and release load request
        public void DestroyAll()
        {
            ClearAll();
            for(int i = 0; i < pool.Count; i++)
            {
                if (pool[i] == null) continue;
                GameObject.Destroy(pool[i].gameObject);
            }
            for (int i = 0; i < inUsePool.Count; i++)
            {
                if (inUsePool[i] == null) continue;
                GameObject.Destroy(inUsePool[i].gameObject);
            }
            if (loadRequest.IsValid())
            {
                
                Addressables.Release(loadRequest);
            }
        }
    }

}