using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SceneManger
{
    public abstract class BaseSceneController : MonoBehaviour, ISceneController
    {
        [SerializeField] private BaseScenePresenter m_ScenePresenter;

        public BaseSceneManager SceneManager { get; private set; }
        public BaseScenePresenter ScenePresenter => m_ScenePresenter;

        public bool IsInitialized { get; private set; }
        public bool IsEnter { get; private set; }

        public void Init()
        {
            SceneManager = BaseSceneManager.Instance;
            ScenePresenter.Initialize(this);
            IsInitialized = true;
        }

        /// <summary>
        /// Preload asset need for scene
        /// </summary>
        /// <returns></returns>
        public abstract UniTask RequestAssets();

        public void Enter()
        {
            OnEnter();
        }

        protected virtual void OnEnter()
        {
            IsEnter = true;
        }

        public virtual UniTask Exit(bool reload)
        {
            IsEnter = false;
            IsInitialized = false;
            SceneManager.Clear();
            return UniTask.CompletedTask;
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

        /// <summary>
        /// Preload asset to asynchorous
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        protected UniTask RequestAsset<T>(string id, string path) where T : UnityEngine.Object
        {
            Logger.Log("Request Load: " + id + " --- " + path);

            var asset = SceneManager.RequestAsync<T>(id, path);
            return asset;
        }

        /// <summary>
        /// Get asset from loaded asset (prefab, image, sound,..)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
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
