using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ui.View;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SceneManger
{
    public class BaseScenePresenter : PanelManager
    {
        public static new BaseScenePresenter Instance => PanelManager.Instance as BaseScenePresenter;

        public BaseSceneController SceneController { private set; get; }

        public virtual void Initialize(BaseSceneController sceneController)
        {
            SceneController = sceneController; 
        }

        
        public virtual void Execute()
        {
            foreach (var presenter in panels)
            {
                presenter.Execute();
            }
        }

        public virtual void Exit()
        {
            foreach (var presenter in panels)
            {
                presenter.Close();
            }
        }

        public new void Create(Type type, System.Action<Panel> onLoaded)
        {
            Create(type.ToString(), onLoaded);
        }
        public new void Create(string type, System.Action<Panel> onLoaded)
        {
            CreateAsync(type).ContinueWith(onLoaded).Forget();
        }
        public new async UniTask<Panel> CreateAsync(string type)
        {
            AsyncOperationHandle<GameObject> op = Addressables.InstantiateAsync(type, transform);
            await op;
            Panel panel = op.Result.GetComponent<Panel>();
            panel.SetPanelMgr(this);
            panel.Prepare();
            panel.PostInit();
            return panel;
        }
        public new async UniTask<Panel> CreateAsync(Type type)
        {
            return await CreateAsync(type.ToString());
        }
        public new async UniTask<TPanel> CreateAsync<TPanel>() where TPanel : Panel
        {
            var type = typeof(TPanel);
            AsyncOperationHandle<GameObject> op = Addressables.InstantiateAsync(type.ToString(), transform);
            await op;
            TPanel panel = op.Result.GetComponent<TPanel>();
            panel.SetPanelMgr(this);
            panel.Prepare();
            panel.PostInit();
            return panel;
        }
        public new async UniTask<TPanel> CreateAsync<TPanel>(string address) where TPanel : Panel
        {
            AsyncOperationHandle<GameObject> op = Addressables.InstantiateAsync(address, transform);
            await op;
            TPanel panel = op.Result.GetComponent<TPanel>();
            panel.SetPanelMgr(this);
            panel.Prepare();
            panel.PostInit();
            return panel;
        }
        public new async UniTask<TPanel> CreateAsync<TPanel>(string address, System.Action<float> onLoading, System.Action onLoaded) where TPanel : Panel
        {
            AsyncOperationHandle<GameObject> op = Addressables.InstantiateAsync(address, transform);
            while (!op.IsDone)
            {
                onLoading?.Invoke(op.PercentComplete);
                await UniTask.Yield();
            }
            TPanel panel = op.Result.GetComponent<TPanel>();
            panel.SetPanelMgr(this);
            panel.Prepare();
            panel.PostInit();

            onLoaded?.Invoke();
            return panel;
        }

        public new TPanel Create<TPanel>(GameObject prefab) where TPanel : Panel
        {
            var op = Instantiate(prefab, transform);
            TPanel panel = op.GetComponent<TPanel>();
            panel.SetPanelMgr(this);
            panel.Prepare();
            panel.PostInit();
            return panel;
        }

    }
    public class BaseScenePresenter<T> : BaseScenePresenter where T : BaseSceneController
    {
        public new T SceneController => (T)base.SceneController;

        public override void Initialize(BaseSceneController sceneController)
        {
            Init();
            base.Initialize(sceneController);
            Initialize((T)sceneController);
        }

        public virtual void Initialize(T sceneController)
        {
        }
    }
}
