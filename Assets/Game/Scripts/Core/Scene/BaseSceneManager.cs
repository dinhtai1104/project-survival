using com.assets.loader.addressables;
using com.mec;
using Cysharp.Threading.Tasks;
using Pool;
using SceneManger.Loading;
using SceneManger.Preloader;
using SceneManger.Transition;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SceneManger
{
    public class BaseSceneManager : MonoSingleton<BaseSceneManager>
    {
        [TabGroup("A", "default")]
        [SerializeField, ClassExtends(typeof(BaseSceneController))]
        private protected ClassTypeReference _startingScene;

        [TabGroup("A", "default")]
        [SerializeField]
        private SceneData[] _sceneData;

        private protected Dictionary<Type, SceneData> _sceneLookup;
        [ShowInInspector]
        private ISceneController _currentSceneController;
        private SceneData _currentSceneData;
        private SceneData _nextSceneData;


        protected bool IsInitialized;
        private bool _isLoading;

        private AssetPreloader m_AssetPreloader;
        public SceneData CurrentSceneData => _currentSceneData;

        #region UNITY METHOD
        protected virtual void Initialize()
        {
            m_AssetPreloader = new AssetPreloader(new AddressableAssetLoader());
            _sceneLookup = new Dictionary<Type, SceneData>();

            foreach (var mapping in _sceneData)
            {
                if (mapping == null)
                {
                    Debug.LogError("Scene mapping is misisng. Please check scene data");
                    continue;
                }

                if (mapping.SceneType == null)
                {
                    Debug.LogError("Scene Type is missing. Please check scene controller type of " + mapping.SceneName);
                    continue;
                }

                if (_sceneLookup.ContainsKey(mapping.SceneType))
                {
                    Debug.LogError(
                        $"Duplicate scene data type between {_sceneLookup[mapping.SceneType].SceneName} and {mapping.SceneName}");
                    continue;
                }

                _sceneLookup.Add(mapping.SceneType, mapping);
            }
        }

        public void InitGame()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                _EnterFirstSceneAsync().Forget();
            }
        }

        public void AddAsyncTask(UniTask task)
        {
            m_AssetPreloader.AddAsyncTask(task);
        }

        protected override void Awake()
        {
            base.Awake();
            //InitGame();
        }

        public virtual void Reset()
        {
            Timing.KillCoroutines();
            PoolManager.Instance.DespawnAll();
            IsInitialized = false;
            Messenger.Cleanup(true);
        }


        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (_currentSceneController == null)
            {
                _currentSceneController = FindObjectOfType<BaseSceneController>();
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        protected virtual void Update()
        {
            if (!IsInitialized) return;

            if (_currentSceneController != null && _currentSceneController.IsEnter)
            {
                _currentSceneController.Execute();
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_isLoading)
            {
                OnLoadSceneComplete();
                _isLoading = false;
            }
        }


        #endregion

        private void OnLoadSceneComplete()
        {
        }
        private async UniTask _EnterFirstSceneAsync()
        {
            Initialize();
            _currentSceneData = GetSceneTypeMapping(_startingScene);
            _currentSceneController = GetSceneController(_currentSceneData.SceneType);

            var sceneTransition = CreateSceneTransition(_currentSceneData.EnterTransitionPrefab);

            _currentSceneController.Init();
            await _currentSceneController.RequestAssets();
            _currentSceneController.Enter();
            sceneTransition?.StartTransition();

            OnLoadSceneComplete();
        }

        private SceneData GetSceneTypeMapping(Type type)
        {
            if (HasSceneType(type)) return _sceneLookup[type];

            Debug.LogError("Null mapping at type " + type.Name);
            return null;
        }

        private bool HasSceneType(Type type)
        {
            return _sceneLookup.ContainsKey(type);
        }

        private string GetScenePath(Type type)
        {
            if (HasSceneType(type))
            {
                return _sceneLookup[type].SceneName;
            }

            Debug.LogError("Scene type not found " + type.Name);
            return string.Empty;
        }

        protected string GetScenePath<T>() where T : class, ISceneController
        {
            return GetScenePath(typeof(T));
        }

        private ISceneController GetSceneController(Type type)
        {
            if (!HasSceneType(type))
            {
                Debug.LogError("Scene type not found " + type.Name);
                return null;
            }

            ISceneController controller = null;
            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var go in rootGameObjects)
            {
                controller = go.GetComponent<ISceneController>();

                if (controller != null) break;
            }

            return controller;
        }

        public ISceneController GetSceneController<T>() where T : class, ISceneController
        {
            return GetSceneController(typeof(T));
        }

        public T GetCurrentSceneController<T>() where T : class, ISceneController
        {
            return _currentSceneController as T;
        }

        protected virtual void OnLoadSceneProgress(float percentage)
        {
        }


        private async UniTask LoadSceneAsync(Type type)
        {
            if (!HasSceneType(type))
            {
                Debug.LogError("Scene type does not exist");
                return;
            }

            Time.timeScale = 1f;
            await _currentSceneController.Exit(_currentSceneController.GetType() == type);

            string nextScenePath = GetScenePath(type);
            // Load scene address


            var scene = await Addressables.LoadSceneAsync(nextScenePath, UnityEngine.SceneManagement.LoadSceneMode.Single, false);

            AsyncOperation loadSceneAsync = scene.ActivateAsync();
            InProgressing(type, loadSceneAsync).Forget();
            _isLoading = true;
            
        }

        private async UniTask InProgressing(Type nextSceneType, AsyncOperation loadingAsync)
        {
            loadingAsync.allowSceneActivation = false;

            BaseSceneTransition exitTransition = CreateSceneTransition(_currentSceneData.ExitTransitionPrefab);
            exitTransition?.StartTransition();
            if (exitTransition)
            {
                while (!exitTransition.IsDone)
                {
                    await UniTask.Yield();
                }
            }

            // Get next mapping
            _nextSceneData = GetSceneTypeMapping(nextSceneType);

            BaseLoadingScene loadingScene = StartLoadingScene(_currentSceneData.LoadingScenePrefab);

            while (!loadingAsync.isDone)
            {
                float fillProgress = (loadingAsync.progress + 0.1f) / 2f;
                loadingScene?.LoadingProgress(fillProgress);

                OnLoadSceneProgress(fillProgress);

                if (loadingAsync.progress >= 0.9f)
                {
                    loadingAsync.allowSceneActivation = true;

                    while (_isLoading)
                    {
                        await UniTask.Yield();
                    }

                    // Get new scene controller
                    _currentSceneController = GetSceneController(_nextSceneData.SceneType);
                    _currentSceneController.Init();
                    await _currentSceneController.RequestAssets();

                    // If there is no resource request then fake the loading progress
                    float progress = 0f;
                    float t = 0f; 

                    while (progress < 1f)
                    {
                        t += Time.deltaTime;
                        progress = Mathf.Lerp(0.5f, 1f, t);
                        loadingScene?.LoadingProgress(progress);
                        await UniTask.Yield();
                    }

                    await UniTask.Yield();

                    // Enter current scene
                    _currentSceneData = _nextSceneData;
                    _currentSceneController.Enter();

                    await UniTask.Yield();

                    // Start enter transition after all async work done
                    var enterTransition = CreateSceneTransition(_nextSceneData.EnterTransitionPrefab);

                    // End loading after create transition to prevent flickering
                    loadingScene?.EndLoading();

                    await UniTask.Yield();

                    AsyncOperation unloadOperation = Resources.UnloadUnusedAssets();

                    while (!unloadOperation.isDone)
                    {
                        await UniTask.Yield();
                    }

                    await UniTask.Yield();

                    enterTransition?.StartTransition();
                    break;
                }

                await UniTask.Yield();
            }
        }

        public UniTask LoadSceneAsync<T>() where T : ISceneController
        {
            if (_isLoading) return UniTask.CompletedTask;
            return LoadSceneAsync(typeof(T));
        }

        public UniTask LoadSceneTypeAsync(Type type)
        {
            if (_isLoading) return UniTask.CompletedTask;
            return LoadSceneAsync(type);
        }

        public BaseSceneTransition CreateSceneTransition(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("Missing Loading Transition prefab in scene " + _currentSceneData.SceneName);
                return null;
            }

            var transition = PoolManager.Instance.Spawn(prefab).GetComponent<BaseSceneTransition>();
            return transition;
        }
        private BaseLoadingScene StartLoadingScene(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("Missing Loading Scene prefab in scene " + _currentSceneData.SceneName);
                return null;
            }

            var loadingScene = PoolManager.Instance.Spawn(prefab).GetComponent<BaseLoadingScene>();
            loadingScene.StartLoading(this);
            return loadingScene;
        }

        private UniTask LoadingScene()
        {
            BaseSceneTransition exitTransition = CreateSceneTransition(_currentSceneData.ExitTransitionPrefab);
            exitTransition?.StartTransition();

            _currentSceneController = GetSceneController(_nextSceneData.SceneType);
            _currentSceneController.Init();
            _currentSceneController.RequestAssets();

            return UniTask.CompletedTask;
        }

        public UniTask RequestAsync<T>(string id, string path) where T : Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Asset has null path " + id);
                return UniTask.CompletedTask;
            }

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("Asset has null id " + id);
                return UniTask.CompletedTask;
            }

            return m_AssetPreloader.RequestAsync<T>(id, path);
        }

        public void Request<T>(string id, string path) where T : Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Asset has null path " + id);
                return;
            }

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("Asset has null id " + id);
                return;
            }

            m_AssetPreloader.Request<T>(id, path);
        }

        public T GetAsset<T>(string id) where T : Object
        {
            return m_AssetPreloader.GetAsset<T>(id);
        }

        public void Clear()
        {
            m_AssetPreloader.Clear();
        }

        public bool IsCompletedAsyncTask()
        {
            return m_AssetPreloader.IsAsyncTaskIsCompleted();
        }
    }
}
