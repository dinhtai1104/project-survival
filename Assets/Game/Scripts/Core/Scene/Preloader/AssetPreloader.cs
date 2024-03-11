using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Object = UnityEngine.Object;
namespace SceneManger.Preloader
{
    public class AssetPreloader
    {
        private readonly Dictionary<Type, Dictionary<string, Object>> m_ResourceLookup;
        private IAssetLoader m_AssetLoader;
        private List<UniTask> m_AsyncTask;

        public AssetPreloader(IAssetLoader m_AssetLoader)
        {
            m_AsyncTask = new List<UniTask>();
            this.m_AssetLoader = m_AssetLoader;
            m_ResourceLookup = new Dictionary<Type, Dictionary<string, Object>>();
        }

        public void Clear()
        {
            m_AssetLoader.ReleaseAll();
            m_ResourceLookup.Clear();
        }

        public async UniTask<T> Request<T>(string id, string path) where T : Object
        {
            var assetType = typeof(T);

            if (!m_ResourceLookup.ContainsKey(assetType))
            {
                m_ResourceLookup.Add(assetType, new Dictionary<string, Object>());
            }

            if (m_ResourceLookup[assetType].ContainsKey(id))
            {
                return m_ResourceLookup[assetType][id] as T;
            }

            var obj = await m_AssetLoader.LoadAsync<T>(path).Task;

            if (!m_ResourceLookup[assetType].ContainsKey(id))
            {
                m_ResourceLookup[assetType].Add(id, obj);
                return m_ResourceLookup[assetType][id] as T;
            }

            return obj;
        }

        public T GetAsset<T>(string id) where T : Object
        {
            var type = typeof(T);

            if (!m_ResourceLookup.ContainsKey(type))
            {
                Debug.LogError("There is no preloaded asset of type " + type);
                return default(T);
            }

            var dict = m_ResourceLookup[type];

            if (!dict.ContainsKey(id))
            {
                Debug.LogError("There is no preloaded asset with id " + id);
                return default(T);
            }

            return (T)dict[id];
        }

        public void AddAsyncTask(UniTask task)
        {
            m_AsyncTask.Add(task);
        }

        public bool IsAsyncTaskIsCompleted()
        {
            if (m_AsyncTask.Count == 0) return true;
            foreach (var task in m_AsyncTask)
            {
                if (task.Status != UniTaskStatus.Succeeded) return false;
            }

            m_AsyncTask.Clear();
            return true;
        }
    }
}
