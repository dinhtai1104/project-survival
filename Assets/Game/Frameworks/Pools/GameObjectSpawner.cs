using Cysharp.Threading.Tasks;
using Game.GameActor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Pool
{
    public enum EPool
    {
        Pernament,Temporary,Projectile
    }
    public class GameObjectSpawner : SpawnerBase<PoolObject>
    {
        public static GameObjectSpawner Instance;
        void Start()
        {
            Instance = this;
            for(int i = 0; i < System.Enum.GetNames(typeof(EPool)).Length; i++)
            {
                pools.Add((EPool)i,new Dictionary<string, ObjectCollection>());
            }
        }
        [ShowInInspector]
        protected Dictionary<EPool, Dictionary<string, ObjectCollection>> pools = new Dictionary<EPool, Dictionary<string, ObjectCollection>>();
        bool ContainsKey(string id)
        {
            foreach(var collection in pools)
            {
                if (collection.Value.ContainsKey(id))
                {
                    return true;
                }
            }
            return false;
        }
        ObjectCollection Get(string id)
        {
            foreach (var collection in pools)
            {
                if (collection.Value.ContainsKey(id))
                {
                    return collection.Value[id];
                }
            }
            return null;
        }
        void Add(EPool poolType,string id,ObjectCollection collection)
        {
            pools[poolType].Add(id, collection);
        }



        //
        public override async UniTask<PoolObject> GetAsync(string id, EPool poolType = EPool.Temporary)
        {
            ObjectCollection collection=null;
            //creat new object collection
            if (!this.ContainsKey(id) )
            {
                collection = new ObjectCollection(Addressables.LoadAssetAsync<Object>(id));
                this.Add(poolType,id, collection);
            }
            //if load request is null, create new request
            else if (this.ContainsKey(id) && !Get(id).loadRequest.IsValid())
            {
                collection = this.Get(id);
                collection.loadRequest=(Addressables.LoadAssetAsync<Object>(id));
            }
            else
            {

                collection = this.Get(id);
            }
            await collection.Load();

            return collection.Get(transform);

        }
        public override void Get(string id,System.Action<PoolObject> onLoaded, EPool poolType = EPool.Temporary)
        {
            GetAsync(id,poolType).ContinueWith(onLoaded).Forget();
        }

        public override void Destroy(bool everything)
        {
            foreach (var pool in pools)
            {
                if (!everything &&pool.Key == EPool.Pernament) continue;
                Dictionary<string, ObjectCollection> collections = pool.Value;
                foreach (var collection in collections)
                {
                    collection.Value.DestroyAll();
                }
                pool.Value.Clear();
            }
        }
      
        public override void ClearAll()
        {
            foreach (var pool in pools)
            {
                foreach (var collection in pool.Value.Values)
                {
                    collection.ClearAll();
                }
            }
        }

        public override void ClearPool(EPool poolType,bool useEffect=false)
        {
            foreach (var collection in pools[poolType].Values)
            {
                collection.ClearAll(useEffect);
            }
        }


    }

}