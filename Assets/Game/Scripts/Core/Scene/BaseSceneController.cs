using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using SceneManger.Preloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneManger
{
    public abstract class BaseSceneController : MonoBehaviour, ISceneController
    {
        public BaseSceneManager SceneManager { get; private set; }

        public bool IsInitialized { get; private set; }
        public bool IsEnter { get; private set; }

        public void Init()
        {
            SceneManager = BaseSceneManager.Instance;
            IsInitialized = true;
        }

        public abstract UniTask RequestAssets();

        public void Enter()
        {
            OnEnter();
        }

        protected virtual void OnEnter()
        {

        }

        public virtual void Exit(bool reload)
        {
            IsEnter = false;
            IsInitialized = false;
            SceneManager.Clear();
            // Clear scene
        }

        public virtual void Execute()
        {
            if (!IsInitialized) return;
        }

        public virtual void Focus()
        {

        }
        public virtual void Unfocus()
        {

        }

        protected UniTask RequestAsset<T>(string id, string path) where T : UnityEngine.Object
        {
            Logger.Log("Request Load: " + id + " --- " + path);

            var asset = SceneManager.Request<T>(id, path);
            return asset;
        }


        public T GetRequestedAsset<T>(string id) where T : UnityEngine.Object
        {
            return SceneManager.GetAsset<T>(id);
        }

    }
}

namespace SceneManger
{
    /// <summary>
    /// Superclass of game manager
    /// </summary>
    /// <typeparam name="T">Conrete type of game manager</typeparam>
    public abstract class BaseSceneController<T> : BaseSceneController where T : BaseSceneManager
    {
        public new T SceneManager => (T)base.SceneManager;
    }
}
